using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDo_Api.Entities.Configurations;

/// <summary>
/// Class responsible for configuration of entity in database.
/// </summary>
public class ToDoConfiguration : IEntityTypeConfiguration<ToDo>
{
    public void Configure(EntityTypeBuilder<ToDo> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Expire)
            .IsRequired();
    }
}
