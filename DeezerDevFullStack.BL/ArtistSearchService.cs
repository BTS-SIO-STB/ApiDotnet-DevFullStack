using DeezerDevFullStack.DTO;
using System.Text.Json;
using System.Net.Http;

namespace DeezerDevFullStack.BL
{
    public interface IServiceDeezer
    {
        Task<IEnumerable<Artist>> SearchArtists(string name);
    }
    
    public class ArtistSearchService : IServiceDeezer
    {
        private readonly HttpClient _httpClient;

        public ArtistSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Artist>> SearchArtists(string name)
        {
            var response = await _httpClient.GetAsync($"https://api.deezer.com/search/artist?q={name}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);

            if (!jsonDocument.RootElement.TryGetProperty("data", out var dataElement))
            {
                return Enumerable.Empty<Artist>();
            }

            var artists = dataElement.EnumerateArray()
                .Select(element => new Artist
                {
                    Id = element.GetProperty("id").GetInt32(),
                    Name = element.GetProperty("name").GetString()
                }).ToList();

            return artists;
        }
    }
}