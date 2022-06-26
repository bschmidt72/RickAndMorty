using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using webapi.Services;
using Xunit;

namespace webapiTest.Services
{
    public class CharacterApiServiceTest
    {
        
        private CharacterApiService SetupService()
        {
            var loggerMock = new Mock<ILogger<CharacterApiService>>();
            var logger = loggerMock.Object;
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.test.json", false, true);
            IConfiguration configuration = builder.Build();
            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Scheme).Returns("https");
            request.SetupGet(x => x.Host).Returns(new HostString("localhost", 5001));
            request.SetupGet(x => x.PathBase).Returns("");
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(context.Object);
            var httpContextAccessor = httpContextAccessorMock.Object;
            return new CharacterApiService(logger, configuration, httpContextAccessor);
        }

        [Fact]
        public void TestGetAllPage1()
        {
            var service = SetupService();
            var result = service.GetAllCharacters(null);
            Assert.NotNull(result);
            Assert.NotNull(result["info"]);
            Assert.Equal(42, (int?) result["info"]?["pages"]);
            Assert.Equal(826, (int?) result["info"]?["count"]);
            Assert.Equal("https://localhost:5001/api/character/?page=2", result["info"]?["next"]);
            Assert.False(((JValue)result["info"]?["prev"]).HasValues);
            var results = (JArray?) result["results"];
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Antenna Rick", results?[18]["name"]);
        }
        
        [Fact]
        public void TestGetAllPage23()
        {
            var service = SetupService();
            var result = service.GetAllCharacters(23);
            Assert.NotNull(result);
            Assert.NotNull(result["info"]);
            Assert.Equal(42, (int?) result["info"]?["pages"]);
            Assert.Equal(826, (int?) result["info"]?["count"]);
            Assert.Equal("https://localhost:5001/api/character/?page=24", result["info"]?["next"]);
            Assert.Equal("https://localhost:5001/api/character/?page=22", result["info"]?["prev"]);
            var results = (JArray?) result["results"];
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Funny Songs Presenter", results?[16]["name"]);
        }
        
        [Fact]
        public void TestQueryRickPage1()
        {
            var service = SetupService();
            var query = "Rick";
            var result = service.QueryCharacters(query, null);
            Assert.NotNull(result);
            Assert.NotNull(result["info"]);
            Assert.Equal(6, (int?) result["info"]?["pages"]);
            Assert.Equal(107, (int?) result["info"]?["count"]);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=2", result["info"]?["next"]);
            Assert.False(((JValue)result["info"]?["prev"]).HasValues);
            var results = (JArray?) result["results"];
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Insurance Rick", results?[16]["name"]);
        }
        
        [Fact]
        public void TestQueryRickPage3()
        {
            var service = SetupService();
            var query = "Rick";
            var result = service.QueryCharacters(query, 3);
            Assert.NotNull(result);
            Assert.NotNull(result["info"]);
            Assert.Equal(6, (int?) result["info"]?["pages"]);
            Assert.Equal(107, (int?) result["info"]?["count"]);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=2", result["info"]?["prev"]);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=4", result["info"]?["next"]);
            var results = (JArray?) result["results"];
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Toxic Rick", results?[7]["name"]);
        }

        [Fact]
        public void TestGetCharacter555()
        {
            var service = SetupService();
            var result = service.GetCharacter(555);
            Assert.Equal("Randotron", result["name"]);
            Assert.Equal(555, (int?)result["id"]);
        }
    }
}