namespace DeezerDevFullStack.DTO;

public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // Navigation property to a list of Songs
    public List<Song> Songs { get; set; }
}