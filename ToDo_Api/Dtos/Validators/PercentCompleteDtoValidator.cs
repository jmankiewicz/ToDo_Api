using FluentValidation;

namespace ToDo_Api.Dtos.Validators;

/// <summary>
/// Class responsible for validation data of <see cref="PercentCompleteDto"/> items.
/// </summary>
public class PercentCompleteDtoValidator : AbstractValidator<PercentCompleteDto>
{
    public PercentCompleteDtoValidator()
    {
        RuleFor(p => p.PercentComplete)
            .InclusiveBetween(0, 100);
    }
}