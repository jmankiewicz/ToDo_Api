using FluentValidation.TestHelper;
using ToDo_Api.Dtos;
using ToDo_Api.Dtos.Validators;

namespace ToDo_Api.IntegrationTests;

/// <summary>
/// Class responsible for integration tests of <see cref="CreateToDoDtoValidator"/>
/// </summary>
public class CreateToDoDtoValidatorTests
{
    [Theory]
    [InlineData("First updated title", "First updated description")]
    [InlineData("Second updated title", "Second updated description")]
    [InlineData("Third updated title", "Third updated description")]
    public async Task Validate_ForValidData_ReturnsSuccess(string title, string description)
    {
        var validator = new CreateToDoDtoValidator();
        var createToDoDto = GetCreateToDoDto(title, description);

        var result = await validator.TestValidateAsync(createToDoDto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("d")]
    [InlineData("de")]
    [InlineData("des")]
    public async Task Validate_ForInvalidDescriptionData_ReturnsSuccess(string description)
    {
        var validator = new CreateToDoDtoValidator();
        var createToDoDto = GetCreateToDoDto("ToDo", description);

        var result = await validator.TestValidateAsync(createToDoDto);

        result.ShouldHaveValidationErrorFor("Description");
        result.ShouldNotHaveValidationErrorFor("Title");
    }

    [Theory]
    [InlineData("")]
    [InlineData("T")]
    [InlineData("To")]
    public async Task Validate_ForInvalidTitleData_ReturnsSuccess(string title)
    {
        var validator = new CreateToDoDtoValidator();
        var createToDoDto = GetCreateToDoDto(title, "Description");

        var result = await validator.TestValidateAsync(createToDoDto);

        result.ShouldHaveValidationErrorFor("Title");
        result.ShouldNotHaveValidationErrorFor("Description");
    }

    private static CreateToDoDto GetCreateToDoDto(string title, string description)
        => new() { Title = title, Description = description };
}
