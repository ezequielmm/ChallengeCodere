using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.Models;

namespace Challenge.Repositories
{
    public interface IShowRepository
    {
        Task<IEnumerable<Show>> GetAllShowsAsync();
        Task<Show> GetShowByIdAsync(int id);
        Task AddShowAsync(Show show);
        Task UpdateShowAsync(Show show);
        Task DeleteShowAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
