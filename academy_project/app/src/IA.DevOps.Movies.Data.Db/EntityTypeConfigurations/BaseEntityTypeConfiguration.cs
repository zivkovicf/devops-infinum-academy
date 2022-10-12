using IA.DevOps.Movies.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IA.DevOps.Movies.Data.Db.EntityTypeConfigurations
{
    internal class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : Base
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);

            builder
                .Property(x => x.UpdatedAt)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
