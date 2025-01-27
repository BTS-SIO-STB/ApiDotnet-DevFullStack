using DeezerDevFullStack.BL;
using DeezerDevFullStack.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DeezerDevFullStack.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeezerController : ControllerBase
    {
        private readonly IServiceDeezer _artistService;

        public DeezerController(IServiceDeezer artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Artist>> SearchArtists(string name)
        {
            return await _artistService.SearchArtists(name);
        }

        [HttpGet("songs/{artistId}")]
        public async Task<IEnumerable<Song>> GetSongsByArtistId(int artistId)
        {
            return await _artistService.GetSongsByArtistId(artistId);
        }
    }
}