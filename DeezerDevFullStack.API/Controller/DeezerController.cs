using DeezerDevFullStack.BL;
using DeezerDevFullStack.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DeezerDevFullStack.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeezerController : ControllerBase
    {
        private readonly IServiceDeezer _artistSearchService;

        public DeezerController(IServiceDeezer artistSearchService)
        {
            _artistSearchService = artistSearchService;
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Artist>> SearchArtists(string name)
        {
            return await _artistSearchService.SearchArtists(name);
        }
        
    }
}