using Microsoft.EntityFrameworkCore;
using DeezerDevFullStack.DTO;

namespace DeezerDevFullStack.DAL
{
    public class DeezerDbContext : DbContext
    {
        public DeezerDbContext(DbContextOptions<DeezerDbContext> options) : base(options) { }

        // Define DbSets for your entities (tables)
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<Song> Song { get; set; }
        public DbSet<Artist> Artist { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure Artist ID is not auto-incrementing
            modelBuilder.Entity<Artist>()
                .Property(a => a.Id)
                .ValueGeneratedNever(); // Disable auto-increment
            
            modelBuilder.Entity<Song>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();

            // Configure the relationship between Song and Artist
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Artist)          // A Song has one Artist
                .WithMany(a => a.Songs)         // An Artist can have many Songs
                .HasForeignKey(s => s.ArtistId) // The foreign key in Song is ArtistId
                .OnDelete(DeleteBehavior.Cascade); // Optional: specify delete behavior (cascade, set null, etc.)
            
            modelBuilder.Entity<Playlist>().HasKey(p => p.Name); // Optional: Use Name as a unique identifier

            // Configure the relationship between Playlist and Song
            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.Songs)  // A Playlist contains many Songs
                .WithOne()              // A Song can belong to the Playlist
                .OnDelete(DeleteBehavior.Cascade); // Optional: configure delete behavio
        }
    }
}