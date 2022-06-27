# build sdm app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /sourceapi

COPY webapi/*.csproj .

RUN dotnet restore

RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_17.x | bash - && \
    apt-get install -y build-essential nodejs

COPY webapi/ .
COPY docker/nginx/ .
COPY docker/supervisord .

RUN dotnet publish -c release -o /api

WORKDIR /sourcespa

COPY spa/ .
RUN mv ./src/config.prod.ts ./src/config.ts

RUN npm install && \
    npm run build -- && \
    mkdir /spa && \
    cp -R ./dist/* /spa
    
# final stage/image
FROM ubuntu:20.04

ENV TERM linux

RUN apt-get update \
  && apt-get install -y libgdiplus \
  && apt-get install -y wget \
  && apt-get install -y nginx \
  && rm -rf /var/lib/apt/lists/*

RUN wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb

RUN apt-get update; \
  apt-get install -y apt-transport-https && \
  apt-get update && \
  apt-get install -y aspnetcore-runtime-6.0

COPY ./docker/nginx/nginx.conf /etc/nginx

COPY --from=build /api /api
RUN mkdir /api/log

COPY --from=build /spa /spa

WORKDIR /app

#supervisor
RUN apt-get update && apt-get install -y supervisor
COPY docker/supervisord/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

EXPOSE 80
EXPOSE 5000

ENTRYPOINT ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]
