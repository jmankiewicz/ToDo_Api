using ToDo_Api.Entities;

namespace ToDo_Api.Dtos;

/// <summary>
/// Represents the data transfer object <see cref="ToDo.CompletePercent"/> property in <see cref="ToDo"/> item.
/// </summary>
public class PercentCompleteDto
{
    public double PercentComplete { get; set; }
}
