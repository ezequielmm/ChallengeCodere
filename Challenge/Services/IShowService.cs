using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.DTOs;
using Challenge.Models;

namespace Challenge.Services
{
    public interface IShowService
    {
        Task FetchAndStoreShowsAsync();
        Task<IEnumerable<Show>> GetAllShowsAsync();
        Task<Show> GetShowByIdAsync(int id);
        Task AddShowAsync(Show show);
        Task UpdateShowAsync(Show show);
        Task DeleteShowAsync(int id);


        Task<IEnumerable<ShowResponseDto>> GetAllShowsDtoAsync();
        Task<ShowResponseDto> GetShowDtoByIdAsync(int id);
    }
}
