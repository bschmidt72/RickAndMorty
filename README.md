# RickAndMorty Character Search
## Technologies used
* Vue.js vite application (spa) for the frontend
* .NET Core 6 Microservice for the backend
* LiteDB embedded NOSQL database for persistent storage
* Nginx for production frontend hosting and forwarding
* Caching is done via two simple LRU/LFU Cache implementations

## How To Configure Fronted
You can configure the frontend by editing the config.ts (config.prod.ts for production) file in spa/src.
```json lines
apiUrl: "https://localhost:5001/api/"
```
Change this to use another backend for the character search.

## How To Configure Backend
You can configure the backend by editing the appsettings.Development.json (appsettings.json for production) file in folder webapi.
### Caching
```json lines
"EnableCaching": true
```
Set this to false to disable caching.

### Database
```json lines
"UseDb": true
```
Set this to false to disable persistent storage (LiteDB). The requests will then be forwarded
to rickandmortyapi.com to fetch the data.


## How To Run
### Production
* Install Docker
* Execute this from the root directory to build the image:
```
docker build -t ram:latest .
```
* Start the docker container:
```
docker run -p 80:80 ram:latest
```
* Open browser and go to 
[http://localhost](http://localhost)

### Development
* Install: DotNetCore SDK 6, NodeJs ^16.15.1

* Execute this from the root directory, which will start frontend and backend simultaneously (Windows):
```
devRun.cmd
```
* Open browser and go to
  [http://localhost:3000](http://localhost:3000)
