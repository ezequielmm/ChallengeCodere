using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Challenge.Data;
using Challenge.DTOs;
using Challenge.Models;
using Challenge.Repositories;
using Challenge.Services;
using Xunit;

namespace Challenge.Tests.Services
{
    public class ShowServiceTests
    {
        private readonly Mock<IShowRepository> _showRepositoryMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly ShowService _showService;

        public ShowServiceTests()
        {
            _showRepositoryMock = new Mock<IShowRepository>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);

            _showService = new ShowService(_showRepositoryMock.Object, context, _httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task GetAllShowsDtoAsync_ReturnsShowDtos()
        {
            var shows = new List<Show>
            {
                new Show
                {
                    Id = 1,
                    Name = "Test Show",
                    Language = "English",
                    Genres = new List<Genre>
                    {
                        new Genre { Name = "Drama" }
                    },
                    Network = new Network
                    {
                        Id = 1,
                        Name = "Test Network",
                        Country = new Country
                        {
                            Code = "US",
                            Name = "United States",
                            Timezone = "America/New_York"
                        }
                    },
                    Rating = new Rating
                    {
                        Id = 1,
                        Average = 8.5
                    },
                    Externals = new Externals
                    {
                        Id = 1,
                        Imdb = "tt1234567",
                        Tvrage = 12345,
                        Thetvdb = 67890
                    }
                }
            };

            _showRepositoryMock.Setup(repo => repo.GetAllShowsAsync()).ReturnsAsync(shows);

            var result = await _showService.GetAllShowsDtoAsync();

            result.Should().HaveCount(1);
            var showDto = result.First();
            showDto.Name.Should().Be("Test Show");
            showDto.Genres.Should().Contain("Drama");
            showDto.Network.Name.Should().Be("Test Network");
            showDto.Rating.Average.Should().Be(8.5);
            showDto.Externals.Imdb.Should().Be("tt1234567");
        }

        [Fact]
        public async Task GetShowDtoByIdAsync_ReturnsShowDto()
        {
            var show = new Show
            {
                Id = 1,
                Name = "Test Show",
                Language = "English"
            };

            _showRepositoryMock.Setup(repo => repo.GetShowByIdAsync(1)).ReturnsAsync(show);

            var result = await _showService.GetShowDtoByIdAsync(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Test Show");
        }

        [Fact]
        public async Task FetchAndStoreShowsAsync_FetchesDataFromApi()
        {
            var showDtos = new List<ShowDto>
            {
                new ShowDto
                {
                    Id = 1,
                    Name = "API Show",
                    Language = "English",
                    Genres = new List<string> { "Drama" }
                }
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(showDtos))
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
            await _showService.FetchAndStoreShowsAsync();

            _showRepositoryMock.Verify(repo => repo.GetShowByIdAsync(It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
