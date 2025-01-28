using DeezerDevFullStack.DTO;
using DeezerDevFullStack.DAL;
using System.Text.Json;

namespace DeezerDevFullStack.BL
{
    public interface IServiceDeezer
    {
        Task<IEnumerable<Artist>> SearchArtists(string name);
        Task<IEnumerable<Song>> GetSongsByArtistId(int artistId);
        Task SaveSongToPlaylist(int songId);
        Task<IEnumerable<Song>> GetSongsFromPlaylist();
    }
    
    public class ArtistService : IServiceDeezer
    {
        private readonly HttpClient _httpClient;
        private readonly IDataDeezer _dataDeezer;

        public ArtistService(HttpClient httpClient, IDataDeezer dataDeezer)
        {
            _httpClient = httpClient;
            _dataDeezer = dataDeezer;
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

            await _dataDeezer.SaveArtists(artists);
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
                            Title = element.GetProperty("title").GetString(),
                            ArtistId = artistId,
                            SongUrl = element.GetProperty("link").GetString()
                        };
                    }
                    return null;
                })
                .Where(song => song != null && !string.IsNullOrEmpty(song.Title))
                .ToList();

            var savedSongs = await _dataDeezer.SaveSongs(songs);
            return savedSongs;
        }
        
        public async Task SaveSongToPlaylist(int songId)
        {
            await _dataDeezer.SaveSongToPlaylist(songId);
        }
        
        public async Task<IEnumerable<Song>> GetSongsFromPlaylist()
        {
            return await _dataDeezer.GetSongsFromPlaylist();
        }
    }
}