using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Challenge.Data;
using Challenge.DTOs;
using Challenge.Models;
using Challenge.Repositories;
using Challenge.DTO;

namespace Challenge.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ShowService(
            IShowRepository showRepository,
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory)
        {
            _showRepository = showRepository;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task FetchAndStoreShowsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var showsUrl = "http://api.tvmaze.com/shows";

            var response = await client.GetAsync(showsUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var showsFromApi = JsonSerializer.Deserialize<List<ShowDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                var existingGenres = new Dictionary<string, Genre>();
                var existingCountries = new Dictionary<string, Country>();
                var existingNetworks = new Dictionary<int, Network>();

                foreach (var showDto in showsFromApi)
                {
                    var existingShow = await _showRepository.GetShowByIdAsync(showDto.Id);
                    if (existingShow != null)
                    {
                        continue;
                    }

                    var show = new Show
                    {
                        Id = showDto.Id,
                        Name = showDto.Name,
                        Language = showDto.Language,
                        Genres = new List<Genre>()
                    };

                    if (showDto.Externals != null)
                    {
                        var externals = new Externals
                        {
                            Id = showDto.Id, 
                            Imdb = showDto.Externals.Imdb,
                            Tvrage = showDto.Externals.Tvrage,
                            Thetvdb = showDto.Externals.Thetvdb
                        };

                        show.Externals = externals;
                    }

                    if (showDto.Rating != null)
                    {
                        var rating = new Rating
                        {
                            Id = showDto.Id, 
                            Average = showDto.Rating.Average
                        };

                        show.Rating = rating;
                    }

                    if (showDto.Network != null)
                    {
                        Country country = null;
                        if (showDto.Network.Country != null && !string.IsNullOrEmpty(showDto.Network.Country.Code))
                        {
                            if (!existingCountries.TryGetValue(showDto.Network.Country.Code, out country))
                            {
                                country = await _context.Countries.FindAsync(showDto.Network.Country.Code);
                                if (country == null)
                                {
                                    country = new Country
                                    {
                                        Code = showDto.Network.Country.Code,
                                        Name = showDto.Network.Country.Name,
                                        Timezone = showDto.Network.Country.Timezone
                                    };
                                    _context.Countries.Add(country);
                                    await _context.SaveChangesAsync();
                                }
                                existingCountries[showDto.Network.Country.Code] = country;
                            }
                        }

                        Network network;
                        if (!existingNetworks.TryGetValue(showDto.Network.Id, out network))
                        {
                            network = await _context.Networks.FindAsync(showDto.Network.Id);
                            if (network == null)
                            {
                                network = new Network
                                {
                                    Id = showDto.Network.Id,
                                    Name = showDto.Network.Name,
                                    CountryCode = country?.Code
                                };
                                _context.Networks.Add(network);
                                await _context.SaveChangesAsync();
                            }
                            existingNetworks[showDto.Network.Id] = network;
                        }

                        show.Network = network;
                        show.NetworkId = network.Id;
                    }

                    foreach (var genreName in showDto.Genres)
                    {
                        if (!existingGenres.TryGetValue(genreName, out var genre))
                        {
                            genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                            if (genre == null)
                            {
                                genre = new Genre { Name = genreName };
                                _context.Genres.Add(genre);
                                await _context.SaveChangesAsync();
                            }
                            existingGenres[genreName] = genre;
                        }
                        show.Genres.Add(genre);
                    }

                    _context.Shows.Add(show);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Failed to fetch shows from API.");
            }
        }

        public async Task<IEnumerable<Show>> GetAllShowsAsync()
        {
            return await _showRepository.GetAllShowsAsync();
        }

        public async Task<Show> GetShowByIdAsync(int id)
        {
            return await _showRepository.GetShowByIdAsync(id);
        }

        public async Task AddShowAsync(Show show)
        {
            await _showRepository.AddShowAsync(show);
            await _showRepository.SaveChangesAsync();
        }

        public async Task UpdateShowAsync(Show show)
        {
            await _showRepository.UpdateShowAsync(show);
        }

        public async Task DeleteShowAsync(int id)
        {
            await _showRepository.DeleteShowAsync(id);
        }


        public async Task<IEnumerable<ShowResponseDto>> GetAllShowsDtoAsync()
        {
            var shows = await _showRepository.GetAllShowsAsync();
            return shows.Select(s => MapShowToDto(s)).ToList();
        }

        public async Task<ShowResponseDto> GetShowDtoByIdAsync(int id)
        {
            var show = await _showRepository.GetShowByIdAsync(id);
            if (show == null)
            {
                return null;
            }
            return MapShowToDto(show);
        }

        private ShowResponseDto MapShowToDto(Show s)
        {
            if (s == null)
                return null;

            var dto = new ShowResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Language = s.Language,
                Genres = s.Genres != null ? s.Genres.Select(g => g.Name).ToList() : new List<string>(),
                Network = null,
                Rating = null,
                Externals = null
            };

            if (s.Network != null)
            {
                dto.Network = new NetworkResponseDto
                {
                    Id = s.Network.Id,
                    Name = s.Network.Name,
                    Country = s.Network.Country != null ? new CountryResponseDto
                    {
                        Code = s.Network.Country.Code,
                        Name = s.Network.Country.Name,
                        Timezone = s.Network.Country.Timezone
                    } : null
                };
            }

            if (s.Rating != null)
            {
                dto.Rating = new RatingResponseDto
                {
                    Average = s.Rating.Average
                };
            }

            if (s.Externals != null)
            {
                dto.Externals = new ExternalsResponseDto
                {
                    Imdb = s.Externals.Imdb,
                    Tvrage = s.Externals.Tvrage,
                    Thetvdb = s.Externals.Thetvdb
                };
            }

            return dto;
        }
    }
}
