using Microsoft.EntityFrameworkCore;

namespace ToDo_Api.Entities;

/// <summary>
/// Main class responsible for maintaining all entities in database.
/// </summary>
public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    { 
    }

    public DbSet<ToDo> ToDos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
