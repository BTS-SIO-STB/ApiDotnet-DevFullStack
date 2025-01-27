using DeezerDevFullStack.DTO;

namespace DeezerDevFullStack.DAL
{
    public interface IDataDeezer
    {
        Task<IEnumerable<Artist>> GetArtistsByName(string name);
    }
    
    public class ArtistRepository : IDataDeezer
    {
        
        // This method should query the database for artists matching the provided name.
        // For now, it returns a hardcoded list of artists for demonstration purposes.
        public async Task<IEnumerable<Artist>> GetArtistsByName(string name)
        {
            // Simulate database query with a hardcoded list of artists
            var artists = new List<Artist>
            {
                new Artist { Id = 1, Name = "Artist 1" },
                new Artist { Id = 2, Name = "Artist 2" },
                new Artist { Id = 3, Name = "Artist 3" }
            };

            // Filter the list of artists based on the provided name
            var matchingArtists = artists.FindAll(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            return await Task.FromResult(matchingArtists);
        }
    }
}