using FluentValidation.TestHelper;
using ToDo_Api.Dtos;
using ToDo_Api.Dtos.Validators;

namespace ToDo_Api.IntegrationTests;

/// <summary>
/// Class responsible for integration tests of <see cref="PercentCompleteDtoValidator"/>
/// </summary>
public class PercentCompleteDtoValidatorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public async Task Validate_ForValidData_ReturnsSuccess(double percentComplete)
    {
        var validator = new PercentCompleteDtoValidator();
        var percentCompleteDto = new PercentCompleteDto { PercentComplete = percentComplete };

        var result = await validator.TestValidateAsync(percentCompleteDto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-25)]
    [InlineData(100.1)]
    [InlineData(125)]
    public async Task Validate_ForInvalidData_ReturnsFailure(double percentComplete)
    {
        var validator = new PercentCompleteDtoValidator();
        var percentCompleteDto = new PercentCompleteDto { PercentComplete = percentComplete };

        var result = await validator.TestValidateAsync(percentCompleteDto);

        result.ShouldHaveAnyValidationError();
    }
}
