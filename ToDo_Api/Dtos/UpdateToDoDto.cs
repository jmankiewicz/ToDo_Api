using ToDo_Api.Entities;

namespace ToDo_Api.Dtos;

/// <summary>
/// Represents the data transfer object for updating <see cref="ToDo"/> item.
/// </summary>
public class UpdateToDoDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Expire { get; set; }
}
