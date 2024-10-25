using FluentValidation;

namespace ToDo_Api.Dtos.Validators;

/// <summary>
/// Class responsible for validation data of <see cref="CreateToDoDto"/> items.
/// </summary>
public class CreateToDoDtoValidator : AbstractValidator<CreateToDoDto>
{
    public CreateToDoDtoValidator()
    {
        RuleFor(t => t.Title)
            .Length(3, 60);

        RuleFor(t => t.Description)
            .Length(5, 200);
    }
}