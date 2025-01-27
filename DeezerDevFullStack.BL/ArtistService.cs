using DeezerDevFullStack.DTO;
using System.Text.Json;
using System.Net.Http;

namespace DeezerDevFullStack.BL
{
    public interface IServiceDeezer
    {
        Task<IEnumerable<Artist>> SearchArtists(string name);
        Task<IEnumerable<Song>> GetSongsByArtistId(int artistId);
    }
    
    public class ArtistService : IServiceDeezer
    {
        private readonly HttpClient _httpClient;

        public ArtistService(HttpClient httpClient)
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
        
        public async Task<IEnumerable<Song>> GetSongsByArtistId(int artistId)
        {
            var response = await _httpClient.GetAsync($"https://api.deezer.com/artist/{artistId}/top?limit=10");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);

            if (!jsonDocument.RootElement.TryGetProperty("data", out var dataElement))
            {
                return Enumerable.Empty<Song>();
            }

            var songs = dataElement.EnumerateArray()
                .Select(element =>
                {
                    if (element.TryGetProperty("id", out var idElement) && idElement.TryGetInt32(out var id))
                    {
                        return new Song
                        {
                            Id = id,
                            Title = element.GetProperty("title").GetString(),
                            ArtistId = artistId
                        };
                    }
                    return null;
                })
                .Where(song => song != null)
                .ToList();

            return songs;
        }
    }
}