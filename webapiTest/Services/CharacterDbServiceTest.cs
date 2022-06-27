using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using webapi.Services;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace webapiTest.Services
{
    public class CharacterDbServiceTest
    {
        private IHttpContextAccessor SetupHttContextAccessorMock()
        {
            var request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Scheme).Returns("https");
            request.SetupGet(x => x.Host).Returns(new HostString("localhost", 5001));
            request.SetupGet(x => x.PathBase).Returns("");
            var context = new Mock<HttpContext>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.SetupGet(x => x.HttpContext).Returns(context.Object);
            return httpContextAccessorMock.Object;
        }

        private IConfiguration SetupConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.test.json", false, true);
            return builder.Build();
        }
        
        private CharacterDbService SetupService()
        {
            return new CharacterDbService(SetupApiService(), SetupConfiguration(), SetupHttContextAccessorMock());
        }

        private CharacterApiService SetupApiService()
        {
            var loggerMock = new Mock<ILogger<CharacterApiService>>();
            var logger = loggerMock.Object;
            
            return new CharacterApiService(logger, SetupConfiguration(), SetupHttContextAccessorMock());
        }

        [Fact]
        public void TestGetAllPage1()
        {
            using var service = SetupService();
            var result = service.GetAllCharacters(null);
            Assert.NotNull(result);
            Assert.NotNull(result.Info);
            Assert.Equal(42, result.Info.Pages);
            Assert.Equal(826, result.Info.Count);
            Assert.Equal("https://localhost:5001/api/character/?page=2", result.Info.Next);
            Assert.Null(result.Info.Prev);
            var results = result.Results;
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Antenna Rick", results?[18].Name);
        }
        
        [Fact]
        public void TestGetAllPage23()
        {
            using var service = SetupService();
            var result = service.GetAllCharacters(23);
            Assert.NotNull(result);
            Assert.NotNull(result.Info);
            Assert.Equal(42, result.Info.Pages);
            Assert.Equal(826, result.Info.Count);
            Assert.Equal("https://localhost:5001/api/character/?page=24", result.Info.Next);
            Assert.Equal("https://localhost:5001/api/character/?page=22", result.Info.Prev);
            var results = result.Results;
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Funny Songs Presenter", results?[16].Name);
        }
        
        [Fact]
        public void TestQueryRickPage1()
        {
            using var service = SetupService();
            var query = "Rick";
            var result = service.QueryCharacters(query, null);
            Assert.NotNull(result);
            Assert.NotNull(result.Info);
            Assert.Equal(6, (int?) result.Info.Pages);
            Assert.Equal(107, (int?) result.Info.Count);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=2", result.Info.Next);
            Assert.Null(result.Info.Prev);
            var results = result.Results;
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Insurance Rick", results?[16].Name);
        }
        
        [Fact]
        public void TestQueryRickPage3()
        {
            using var service = SetupService();
            var query = "Rick";
            var result = service.QueryCharacters(query, 3);
            Assert.NotNull(result);
            Assert.NotNull(result.Info);
            Assert.Equal(6, result.Info.Pages);
            Assert.Equal(107, result.Info.Count);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=2", result.Info.Prev);
            Assert.Equal("https://localhost:5001/api/character/?name=Rick&page=4", result.Info.Next);
            var results = result.Results;
            Assert.NotNull(results);
            Assert.Equal(20, results?.Count);
            Assert.Equal("Unknown Rick", results?[7].Name);
        }

        [Fact]
        public void TestGetCharacter555()
        {
            using var service = SetupService();
            var result = service.GetCharacter(555);
            Assert.Equal("Randotron", result.Name);
            Assert.Equal(555, (int?)result.Id);
        }
    }
}