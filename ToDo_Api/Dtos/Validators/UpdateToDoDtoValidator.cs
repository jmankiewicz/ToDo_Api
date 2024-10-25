using FluentValidation;

namespace ToDo_Api.Dtos.Validators;

/// <summary>
/// Class responsible for validation data of <see cref="UpdateToDoDto"/> items.
/// </summary>
public class UpdateToDoDtoValidator : AbstractValidator<UpdateToDoDto>
{
    public UpdateToDoDtoValidator()
    {
        RuleFor(t => t.Title)
            .Length(3, 60);

        RuleFor(t => t.Description)
            .Length(5, 200);
    }
}