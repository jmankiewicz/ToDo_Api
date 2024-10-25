using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;
using ToDo_Api.Models;
using ToDo_Api.Services;

namespace ToDo_Api.UnitTests;

/// <summary>
/// Class responsible for unit tests of <see cref="ToDoService"/>
/// </summary>
public class ToDoServiceTests
{
    private readonly ToDoService _toDoService;

    public ToDoServiceTests()
    {
        var options = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase("ToDo_Db")
            .Options;

        var dbContext = new ToDoDbContext(options);

        var initializationTask = InitializeAsync(dbContext);
        initializationTask.Wait();

        var config = new MapperConfiguration(cfg
            => cfg.AddProfile<AutoMappingProfile>());
        var mapper = config.CreateMapper();

        _toDoService = new ToDoService(dbContext, mapper);
    }

    private static async Task InitializeAsync(ToDoDbContext dbContext)
    {
        if(!await dbContext.ToDos.AnyAsync())
        {
            var seeder = new ToDoSeeder(dbContext);
            await seeder.Seed();
        }
    }

    [Fact]
    public async Task GetAllToDosAsync_ReturnsListOfToDoDto()
    {
        var result = await _toDoService.GetAllToDosAsync();

        result.Should().BeOfType<List<ToDoDto>>();
        result.Should().HaveCountGreaterThan(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetToDoByIdAsync_WithCorrectId_ReturnsToDoDto(int id)
    {
        var result = await _toDoService.GetToDoByIdAsync(id);

        result.Should().BeOfType<ToDoDto>();
        result.Id.Should().Be(id);
    }

    [Theory]
    [InlineData(7777)]
    [InlineData(8888)]
    [InlineData(9999)]
    public async Task GetToDoByIdAsync_WithIncorrectId_ThrowsBadHttpRequestException(int id)
    {
        await _toDoService.Invoking(async s => await s.GetToDoByIdAsync(id))
            .Should()
            .ThrowAsync<BadHttpRequestException>()
            .WithMessage($"ToDo with ID = {id} was not found.");
    }

    [Theory]
    [InlineData(PeriodTime.Today)]
    [InlineData(PeriodTime.NextDay)]
    [InlineData(PeriodTime.CurrentWeek)]
    public void GetIncomingToDos_WithCorrectPeriodTime_ReturnsListOfToDoDto(PeriodTime periodTime)
    {
        var result = _toDoService.GetIncomingToDos(periodTime);

        result.Should().BeOfType<List<ToDoDto>>();
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Theory]
    [InlineData("ToDo", "Description")]
    [InlineData("Test ToDo", "Test Description")]
    public async Task CreateToDoAsync_WithValidData_ReturnsIdOfToDo(string title, string description)
    {
        var createToDoDto = new CreateToDoDto 
        { 
            Title = title, 
            Description = description 
        };
        var result = await _toDoService.CreateToDoAsync(createToDoDto);

        result.Should().BeOfType(typeof(int));
        result.Should().BeGreaterThanOrEqualTo(1);
    }

    [Theory]
    [InlineData("Updated ToDo", "Description", 1)]
    [InlineData("Updated Test ToDo", "Test Description", 2)]
    public async Task UpdateToDoAsync_WithValidData(string title, string description, int id)
    {
        var updateToDoDto = new UpdateToDoDto
        {
            Title = title,
            Description = description,
        };
        await _toDoService.UpdateToDoAsync(updateToDoDto, id);
        
        var result = await _toDoService.GetToDoByIdAsync(id);

        result.Title.Should().Be(title);
        result.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(20, 1)]
    [InlineData(40, 2)]
    [InlineData(60, 3)]
    public async Task SetToDoPercentCompleteAsync_WithValidData(double percentComplete, int id)
    {
        var percentCompleteDto = new PercentCompleteDto { PercentComplete = percentComplete };
        await _toDoService.SetToDoPercentCompleteAsync(percentCompleteDto, id);

        var result = await _toDoService.GetToDoByIdAsync(id);
        result.CompletePercent.Should().Be(percentComplete);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task MarkToDoAsDoneAsync_WithCorrectId(int id)
    {
        await _toDoService.MarkToDoAsDoneAsync(id);

        var result = await _toDoService.GetToDoByIdAsync(id);

        result.CompletePercent.Should().Be(100);
        result.IsDone.Should().BeTrue();
    }
}
