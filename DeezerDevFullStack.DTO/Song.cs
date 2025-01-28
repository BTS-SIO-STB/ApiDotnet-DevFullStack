using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeezerDevFullStack.DTO
{
    public class Song
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int ArtistId { get; set; }
        public string? SongUrl { get; set; }
        public Artist Artist { get; set; }
    }
}