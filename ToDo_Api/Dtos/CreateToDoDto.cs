namespace ToDo_Api.Dtos;

/// <summary>
/// Represents the data transfer object of new <see cref="ToDo"/> item.
/// </summary>
public class CreateToDoDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Expire { get; set; }
}
