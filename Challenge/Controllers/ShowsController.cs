using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Challenge.DTOs;
using Challenge.Models;
using Challenge.Services;

namespace Challenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _showService;

        public ShowsController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowResponseDto>>> GetShows()
        {
            var shows = await _showService.GetAllShowsDtoAsync();
            return Ok(shows);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShowResponseDto>> GetShow(int id)
        {
            var show = await _showService.GetShowDtoByIdAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            return Ok(show);
        }

        [HttpPost]
        public async Task<ActionResult> CreateShow(ShowResponseDto showDto)
        {
            var show = new Show
            {
                Id = showDto.Id,
                Name = showDto.Name,
                Language = showDto.Language,
            };

            await _showService.AddShowAsync(show);
            return CreatedAtAction(nameof(GetShow), new { id = show.Id }, showDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateShow(int id, ShowResponseDto showDto)
        {
            if (id != showDto.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existingShow = await _showService.GetShowByIdAsync(id);
            if (existingShow == null)
            {
                return NotFound();
            }

            existingShow.Name = showDto.Name;
            existingShow.Language = showDto.Language;

            await _showService.UpdateShowAsync(existingShow);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShow(int id)
        {
            var show = await _showService.GetShowByIdAsync(id);
            if (show == null)
            {
                return NotFound();
            }

            await _showService.DeleteShowAsync(id);
            return NoContent();
        }
    }
}
