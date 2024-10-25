using ToDo_Api.Entities;

namespace ToDo_Api.Dtos;

/// <summary>
/// Represents the data transfer object of existing <see cref="ToDo"/> item.
/// </summary>
public class ToDoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Expire { get; set; }
    public double CompletePercent { get; set; }
    public bool IsDone { get; set; }
}
