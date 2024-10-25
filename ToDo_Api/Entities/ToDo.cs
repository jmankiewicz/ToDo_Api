namespace ToDo_Api.Entities;

/// <summary>
/// Representation of entity in database.
/// </summary>
public class ToDo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Expire { get; set; }
    public double CompletePercent { get; set; }
    public bool IsDone { get; set; }
}
