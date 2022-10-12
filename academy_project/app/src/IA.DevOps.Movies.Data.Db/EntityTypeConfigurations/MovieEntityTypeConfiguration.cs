using IA.DevOps.Movies.Contracts.Entities;
using IA.DevOps.Movies.Data.Db.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IA.DevOps.Movies.Data.Db.EntityTypeConfigurations
{
    internal class MovieEntityTypeConfiguration : BaseEntityTypeConfiguration<Movie>
    {
        public new void Configure(EntityTypeBuilder<Movie> builder)
        {
            base.Configure(builder);

            builder
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(DatabaseDefaults.DefaultStringSize);

            builder
                .Property(x => x.ReleasedYear)
                .IsRequired();

            builder
                .Property(x => x.Genre)
                .IsRequired()
                .HasMaxLength(DatabaseDefaults.DefaultStringSize);

            builder
                .Property(x => x.Overview)
                .IsRequired();

            builder
                .Property(x => x.Rating);

            builder
                .HasIndex(nameof(Movie.Title));
        }
    }
}
