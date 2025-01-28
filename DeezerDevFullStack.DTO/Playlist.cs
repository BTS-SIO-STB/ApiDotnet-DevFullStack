namespace DeezerDevFullStack.DTO;

public class Playlist
{
    public string Name { get; set; }
    public List<Song> Songs { get; set; } // Renamed for clarity
    public DateTime SavedAt { get; set; }
}
