using FluentValidation.TestHelper;
using ToDo_Api.Dtos;
using ToDo_Api.Dtos.Validators;

namespace ToDo_Api.IntegrationTests;

/// <summary>
/// Class responsible for integration tests of <see cref="UpdateToDoDtoValidator"/>
/// </summary>
public class UpdateToDoDtoValidatorTests
{
    [Theory]
    [InlineData("First updated title", "First updated description")]
    [InlineData("Second updated title", "Second updated description")]
    [InlineData("Third updated title", "Third updated description")]
    public async Task Validate_ForValidData_ReturnsSuccess(string title, string description)
    {
        var validator = new UpdateToDoDtoValidator();
        var updateToDoDto = GetUpdateToDoDto(title, description);

        var result = await validator.TestValidateAsync(updateToDoDto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("d")]
    [InlineData("de")]
    [InlineData("des")]
    public async Task Validate_ForInvalidDescriptionData_ReturnsFailure(string description)
    {
        var validator = new UpdateToDoDtoValidator();
        var updateToDoDto = GetUpdateToDoDto("Title", description);

        var result = await validator.TestValidateAsync(updateToDoDto);

        result.ShouldHaveValidationErrorFor("Description");
        result.ShouldNotHaveValidationErrorFor("Title");
    }

    [Theory]
    [InlineData("")]
    [InlineData("T")]
    [InlineData("To")]
    public async Task Validate_ForInvalidTitleData_ReturnsFailure(string title)
    {
        var validator = new UpdateToDoDtoValidator();
        var updateToDoDto = GetUpdateToDoDto(title, "Description");

        var result = await validator.TestValidateAsync(updateToDoDto);

        result.ShouldHaveValidationErrorFor("Title");
        result.ShouldNotHaveValidationErrorFor("Description");
    }

    private static UpdateToDoDto GetUpdateToDoDto(string title, string description) 
        => new() { Title = title, Description = description };
}