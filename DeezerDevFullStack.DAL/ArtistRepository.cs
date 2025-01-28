using DeezerDevFullStack.DTO;
using Microsoft.EntityFrameworkCore;

namespace DeezerDevFullStack.DAL
{
    public interface IDataDeezer
    {
        Task<List<Song>> SaveSongs(List<Song> songs);
        Task SaveArtists(List<Artist> artists);
        Task SaveSongToPlaylist(int songId);
        Task<IEnumerable<Song>> GetSongsFromPlaylist();
    }
    
    public class ArtistRepository : IDataDeezer
    {
        private readonly DeezerDbContext _dbContext;

        // Constructor to inject the DbContext
        public ArtistRepository(DeezerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Song>> SaveSongs(List<Song> songs)
        {
            foreach (var song in songs)
            {
                // Check if the artist exists
                var artistExists = await _dbContext.Artist.AnyAsync(a => a.Id == song.ArtistId);
                if (!artistExists)
                {
                    throw new ArgumentException($"Artist with Id {song.ArtistId} does not exist.");
                }
            }

            // Find songs that do not exist in the database (by matching the 'SongUrl' or 'title' and 'artistId' combination)
            var newSongs = songs.Where(song => !_dbContext.Song.Any(s => s.SongUrl == song.SongUrl && s.ArtistId == song.ArtistId)).ToList();

            // Insert new songs if any
            if (newSongs.Any())
            {
                _dbContext.Song.AddRange(newSongs);
                await _dbContext.SaveChangesAsync();
            }

            // Fetch the songs from the database after insertion, ensuring that they have their 'Id' populated
            // Use a separate query to ensure we get the correct songs, using their SongUrl and ArtistId for the filter
            var songUrls = songs.Select(song => song.SongUrl).ToList();
            var artistIds = songs.Select(song => song.ArtistId).ToList();

            var savedSongs = await _dbContext.Song
                .Where(s => songUrls.Contains(s.SongUrl) && artistIds.Contains(s.ArtistId))
                .ToListAsync();

            // Return the saved songs with their IDs populated
            return savedSongs;
        }

        // Save Artists using EF Core
        public async Task SaveArtists(List<Artist> artists)
        {
            var artistEntities = artists.Select(artist => new Artist
            {
                Name = artist.Name,
                Id = artist.Id
            }).ToList();
            _dbContext.Artist.AddRange(artistEntities.Where(artist => !_dbContext.Artist.Any(a => a.Id == artist.Id)));
            await _dbContext.SaveChangesAsync();
        }
        
        // Save Songs to playlist using EF Core
        public async Task SaveSongToPlaylist(int songId)
        {
            // Check if the songId is valid
            if (songId <= 0)
            {
                throw new ArgumentException("Invalid songId", nameof(songId));
            }

            // Retrieve the single playlist, or create it if it doesn't exist
            var playlist = await _dbContext.Playlist.FirstOrDefaultAsync();
            if (playlist == null)
            {
                playlist = new Playlist
                {
                    Name = "Default Playlist",
                    SavedAt = DateTime.UtcNow,
                    Songs = new List<Song>()
                };

                _dbContext.Playlist.Add(playlist);
                await _dbContext.SaveChangesAsync();
            }

            if (playlist.Songs == null)
            {
                playlist.Songs = new List<Song>();
            }

            // Retrieve the song by its Id
            var song = await _dbContext.Song.FindAsync(songId);
            if (song == null)
            {
                throw new ArgumentException($"Song with Id {songId} does not exist.");
            }

            // Add the song to the playlist
            playlist.Songs.Add(song);

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<Song>> GetSongsFromPlaylist()
        {
            var playlist = await _dbContext.Playlist
                .Include(p => p.Songs)
                .FirstOrDefaultAsync(p => p.Name == "Default Playlist");

            if (playlist == null)
            {
                throw new ArgumentException("Default Playlist does not exist.");
            }

            return playlist.Songs;
        }
    }
}