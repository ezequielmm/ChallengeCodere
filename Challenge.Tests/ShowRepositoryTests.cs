using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Challenge.Data;
using Challenge.Models;
using Challenge.Repositories;
using Xunit;

namespace Challenge.Tests.Repositories
{
    public class ShowRepositoryTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) 
                .Options;

            var context = new ApplicationDbContext(options);
            return context;
        }

        [Fact]
        public async Task GetAllShowsAsync_ReturnsAllShows()
        {
            using var context = CreateContext();
            var repository = new ShowRepository(context);
            var show = new Show
            {
                Id = 1,
                Name = "Test Show",
                Language = "English",
                Genres = new List<Genre> { new Genre { Id = 1, Name = "Drama" } },
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
                Externals = new Externals { Id = 1, Imdb = "tt1234567", Tvrage = 12345, Thetvdb = 67890 },
                Rating = new Rating { Id = 1, Average = 8.5 }
            };

            context.Shows.Add(show);
            await context.SaveChangesAsync();

            var shows = await repository.GetAllShowsAsync();

            shows.Should().HaveCount(1);
            shows.First().Name.Should().Be("Test Show");
        }

        [Fact]
        public async Task GetShowByIdAsync_ReturnsShow()
        {
            using var context = CreateContext();
            var repository = new ShowRepository(context);
            var show = new Show { Id = 1, Name = "Test Show", Language = "English" };

            context.Shows.Add(show);
            await context.SaveChangesAsync();

            var result = await repository.GetShowByIdAsync(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Test Show");
        }

        [Fact]
        public async Task AddShowAsync_AddsShow()
        {
            using var context = CreateContext();
            var repository = new ShowRepository(context);
            var newShow = new Show { Id = 2, Name = "New Show", Language = "Spanish" };

            await repository.AddShowAsync(newShow);
            await repository.SaveChangesAsync();

            var result = await repository.GetShowByIdAsync(2);

            result.Should().NotBeNull();
            result.Name.Should().Be("New Show");
        }

        [Fact]
        public async Task UpdateShowAsync_UpdatesShow()
        {
            using var context = CreateContext();
            var repository = new ShowRepository(context);
            var show = new Show { Id = 1, Name = "Test Show", Language = "English" };

            context.Shows.Add(show);
            await context.SaveChangesAsync();

            show.Name = "Updated Show";
            await repository.UpdateShowAsync(show);
            await repository.SaveChangesAsync();

            var updatedShow = await repository.GetShowByIdAsync(1);
            updatedShow.Name.Should().Be("Updated Show");
        }

        [Fact]
        public async Task DeleteShowAsync_DeletesShow()
        {
            using var context = CreateContext();
            var repository = new ShowRepository(context);
            var show = new Show { Id = 1, Name = "Test Show", Language = "English" };

            context.Shows.Add(show);
            await context.SaveChangesAsync();

            await repository.DeleteShowAsync(1);
            await repository.SaveChangesAsync();

            var result = await repository.GetShowByIdAsync(1);
            result.Should().BeNull();
        }
    }
}
