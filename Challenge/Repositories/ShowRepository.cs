using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Challenge.Data;
using Challenge.Models;

namespace Challenge.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Show>> GetAllShowsAsync()
        {
            return await _context.Shows
                .Include(s => s.Network)
                    .ThenInclude(n => n.Country)
                .Include(s => s.Externals)
                .Include(s => s.Rating)
                .Include(s => s.Genres)
                .ToListAsync();
        }

        public async Task<Show> GetShowByIdAsync(int id)
        {
            return await _context.Shows
                .Include(s => s.Network)
                    .ThenInclude(n => n.Country)
                .Include(s => s.Externals)
                .Include(s => s.Rating)
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddShowAsync(Show show)
        {
            _context.Shows.Add(show);
            await Task.CompletedTask;
        }

        public async Task UpdateShowAsync(Show show)
        {
            _context.Shows.Update(show);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShowAsync(int id)
        {
            var show = await GetShowByIdAsync(id);
            if (show != null)
            {
                _context.Shows.Remove(show);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
