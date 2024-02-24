using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using MovieSearch.Application.ServiceImplementation;
using MovieSearch.Domain;
using MovieSearch.Domain.Entities;
using Newtonsoft.Json;
using System.Net;

namespace MovieSearchTesting.ServiceImplementation
{
    public class MovieServiceTests
    {
        [Fact]
        public async Task SearchMovieByTitleAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpClient>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(_ => _.Value).Returns(appSettings);

            var movieService = new MovieService(httpClientFactoryMock.Object, appSettingsMock.Object);

            var fakeApiResponse = ApiResponse<IEnumerable<MovieResult>>.Success(new List<MovieResult>
            {
                new MovieResult { Title = "Fake Movie 1" },
                new MovieResult { Title = "Fake Movie 2" }
            }, "Fake success", 200);

            var fakeHttpContent = new StringContent(JsonConvert.SerializeObject(fakeApiResponse));
            var fakeHttpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = fakeHttpContent
            };

            httpClientMock.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(fakeHttpResponse);

            // Act
            var result = await movieService.SearchMovieByTitleAsync("FakeTitle");

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Fake success", result.Message);
            Assert.Equal(200, result.StatusCode);

            // Additional assertion to check the content
            var movieResults = Assert.IsAssignableFrom<IEnumerable<MovieResult>>(result.Data);
            Assert.Equal(2, movieResults.Count()); 
            Assert.Contains(movieResults, m => m.Title == "Fake Movie 1");
            Assert.Contains(movieResults, m => m.Title == "Fake Movie 2");
        }

        [Fact]
        public async Task SearchMovieByTitleAsync_ShouldReturnNotFoundResponseOnApi404()
        {
            // Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpClient>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appSettings = new AppSettings();
            configuration.GetSection("AppSettings").Bind(appSettings);
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(_ => _.Value).Returns(appSettings);

            var movieService = new MovieService(httpClientFactoryMock.Object, appSettingsMock.Object);

            var fakeHttpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            };

            httpClientMock.Setup(c => c.GetAsync(It.IsAny<string>())).ReturnsAsync(fakeHttpResponse);

            // Act
            var result = await movieService.SearchMovieByTitleAsync("NonExistentTitle");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("No results found", result.Message);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
