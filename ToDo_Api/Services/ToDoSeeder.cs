using ToDo_Api.Entities;

namespace ToDo_Api.Services;

/// <summary>
/// Class responsible for seeding sample data to database.
/// </summary>
public class ToDoSeeder
{
    private readonly ToDoDbContext _dbContext;

    public ToDoSeeder(ToDoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Seed()
    {
        if(await _dbContext.Database.CanConnectAsync())
        {
            var toDos = GetSampleToDos();

            await _dbContext
                .ToDos
                .AddRangeAsync(toDos);
            await _dbContext
                .SaveChangesAsync();
        }
    }

    private static List<ToDo> GetSampleToDos()
    {
        var toDos = new List<ToDo>
        {
            new()
            {
                Title = "ToDo title",
                Description = "ToDo description",
                Expire = DateTime.Today.AddDays(0)
            },
            new()
            {
                Title = "First ToDo title",
                Description = "First ToDo description",
                Expire = DateTime.Today.AddDays(1)
            },
            new()
            {
                Title = "Second ToDo title",
                Description = "Second ToDo description",
                Expire = DateTime.Today.AddDays(2)
            },
            new()
            {
                Title = "Third ToDo title",
                Description = "Third ToDo description",
                Expire = DateTime.Today.AddDays(3)
            },
            new()
            {
                Title = "Fourth ToDo title",
                Description = "Fourth ToDo description",
                Expire = DateTime.Today.AddDays(4)
            },
            new()
            {
                Title = "Fifth ToDo title",
                Description = "Fifth ToDo description",
                Expire = DateTime.Today.AddDays(5)
            },
            new()
            {
                Title = "Sixth ToDo title",
                Description = "Sixth ToDo description",
                Expire = DateTime.Today.AddDays(6)
            },
            new()
            {
                Title = "Seventh ToDo title",
                Description = "Seventh ToDo description",
                Expire = DateTime.Today.AddDays(7)
            },
            new()
            {
                Title = "Eighth ToDo title",
                Description = "Eighth ToDo description",
                Expire = DateTime.Today.AddDays(8)
            },
            new()
            {
                Title = "Ninth ToDo title",
                Description = "Ninth ToDo description",
                Expire = DateTime.Today.AddDays(9)
            },
            new()
            {
                Title = "Tenth ToDo title",
                Description = "Tenth ToDo description",
                Expire = DateTime.Today.AddDays(10)
            },
        };

        return toDos;
    }
}
