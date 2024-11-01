using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Challenge.Controllers;
using Challenge.DTOs;
using Challenge.Models;
using Challenge.Services;
using Xunit;

namespace Challenge.Tests.Controllers
{
    public class ShowsControllerTests
    {
        private readonly Mock<IShowService> _showServiceMock;
        private readonly ShowsController _controller;

        public ShowsControllerTests()
        {
            _showServiceMock = new Mock<IShowService>();
            _controller = new ShowsController(_showServiceMock.Object);
        }

        [Fact]
        public async Task GetShows_ReturnsOkResult_WithListOfShows()
        {
            var showDtos = new List<ShowResponseDto>
            {
                new ShowResponseDto
                {
                    Id = 1,
                    Name = "Test Show"
                }
            };

            _showServiceMock.Setup(svc => svc.GetAllShowsDtoAsync()).ReturnsAsync(showDtos);

            var result = await _controller.GetShows();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedShows = okResult.Value as IEnumerable<ShowResponseDto>;
            returnedShows.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetShow_ReturnsOkResult_WithShow()
        {
            var showDto = new ShowResponseDto
            {
                Id = 1,
                Name = "Test Show"
            };

            _showServiceMock.Setup(svc => svc.GetShowDtoByIdAsync(1)).ReturnsAsync(showDto);

            var result = await _controller.GetShow(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var returnedShow = okResult.Value as ShowResponseDto;
            returnedShow.Should().NotBeNull();
            returnedShow.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetShow_ReturnsNotFound_WhenShowDoesNotExist()
        {
            _showServiceMock.Setup(svc => svc.GetShowDtoByIdAsync(99)).ReturnsAsync((ShowResponseDto)null);

            var result = await _controller.GetShow(99);

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateShow_ReturnsCreatedAtAction()
        {
            var showDto = new ShowResponseDto
            {
                Id = 2,
                Name = "New Show"
            };

            var result = await _controller.CreateShow(showDto);

            var createdAtActionResult = result as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult.ActionName.Should().Be(nameof(_controller.GetShow));
            createdAtActionResult.RouteValues["id"].Should().Be(2);
        }

        [Fact]
        public async Task UpdateShow_ReturnsNoContent()
        {
            var showDto = new ShowResponseDto
            {
                Id = 1,
                Name = "Updated Show"
            };

            _showServiceMock.Setup(svc => svc.GetShowByIdAsync(1)).ReturnsAsync(new Show { Id = 1 });

            var result = await _controller.UpdateShow(1, showDto);

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteShow_ReturnsNoContent()
        {
            _showServiceMock.Setup(svc => svc.GetShowByIdAsync(1)).ReturnsAsync(new Show { Id = 1 });

            var result = await _controller.DeleteShow(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
