using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Data.Db.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace IA.DevOps.Movies.Data.Db
{
    public class MoviesDbContext : DbContext
    {
        public virtual DbSet<Movie> Movies { get; set; } = default!;

        public MoviesDbContext(DbContextOptions<MoviesDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new MovieEntityTypeConfiguration().Configure(modelBuilder.Entity<Movie>());
        }
    }
}
