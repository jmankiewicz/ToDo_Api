using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ToDo_Api.Controllers;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;
using ToDo_Api.IntegrationTests.Helpers;
using ToDo_Api.Models;

namespace ToDo_Api.IntegrationTests;

/// <summary>
/// Class responsible for integration tests of <see cref="ToDoController"/>
/// </summary>
public class ToDoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ToDoControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                        .SingleOrDefault(service 
                            => service.ServiceType == typeof(DbContextOptions<ToDoDbContext>));

                    if(dbContextOptions is not null)
                    {
                        services.Remove(dbContextOptions);

                        services
                            .AddDbContext<ToDoDbContext>(options
                                => options.UseInMemoryDatabase("ToDo_Db"));
                    }
                });
            })
            .CreateClient();
    }

    [Fact]
    public async Task GetAllToDosAsync_ReturnsOkResult()
    {
        var response = await _client.GetAsync("/api/todo/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetToDoByIdAsync_WithCorrectId_ReturnsOkResult(int id)
    {
        var response = await _client.GetAsync($"/api/todo/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(102)]
    [InlineData(103)]
    public async Task GetToDoByIdAsync_WithIncorrectId_ReturnsNotFoundResult(int id)
    {
        var response = await _client.GetAsync($"/api/todo/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(PeriodTime.Today)]
    [InlineData(PeriodTime.NextDay)]
    [InlineData(PeriodTime.CurrentWeek)]
    public async Task GetIncomingToDos_WithCorrectPeriodTime_ReturnsOkResult(PeriodTime periodTime)
    {
        var response = await _client.GetAsync($"/api/todo/incoming/{periodTime}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("First test", "First test description")]
    [InlineData("Second test", "Second test description")]
    [InlineData("Third test", "Third test description")]
    public async Task CreateToDoAsync_WithValidData_ReturnsCreatedResult(string title, string description)
    {
        var newToDo = GetCreateToDoDto(title, description);

        var content = newToDo.ToJsonHttpContent();

        var response = await _client.PostAsync("/api/todo", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("t", "d")]
    [InlineData("to", "de")]
    [InlineData("to", "des")]
    [InlineData("to", "desc")]
    [InlineData("toDo", "desc")]
    [InlineData("to", "description")]
    public async Task CreateToDoAsync_WithInvalidData_ReturnsBadRequestResult(string title, string description)
    {
        var newToDo = GetCreateToDoDto(title, description);

        var content = newToDo.ToJsonHttpContent();

        var response = await _client.PostAsync("/api/todo", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static CreateToDoDto GetCreateToDoDto(string title, string description) 
        => new() { Title = title, Description = description };

    [Theory]
    [InlineData("First updated title", "First updated description")]
    [InlineData("Second updated title", "Second updated description")]
    [InlineData("Third updated title", "Third updated description")]
    public async Task UpdateToDoAsync_WithValidData_ReturnsNoContentResult(string title, string description)
    {
        var updateToDo = GetUpdateToDoDto(title, description);
        var toDoId = 2;

        var content = updateToDo.ToJsonHttpContent();

        var response = await _client.PutAsync($"/api/todo/{toDoId}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("t", "d")]
    [InlineData("to", "de")]
    [InlineData("to", "des")]
    [InlineData("to", "desc")]
    [InlineData("toDo", "desc")]
    [InlineData("to", "description")]
    public async Task UpdateToDoAsync_WithInvalidData_ReturnsBadRequestResult(string title, string description)
    {
        var updateToDo = GetUpdateToDoDto(title, description);
        var toDoId = 2;

        var content = updateToDo.ToJsonHttpContent();

        var response = await _client.PutAsync($"/api/todo/{toDoId}", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static UpdateToDoDto GetUpdateToDoDto(string title, string description) 
        => new() { Title = title, Description = description };

    [Theory]
    [InlineData(4, 0)]
    [InlineData(5, 50)]
    [InlineData(6, 100)]
    public async Task SetToDoPercentCompleteAsync_WithValidData_ReturnsNoContentResult(int id, double percentComplete)
    {
        var percentCompleteDto = new PercentCompleteDto
        {
            PercentComplete = percentComplete
        };

        var content = percentCompleteDto.ToJsonHttpContent();

        var response = await _client.PutAsync($"/api/todo/{id}/percent_complete", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(4, 100.1)]
    [InlineData(5, 105)]
    [InlineData(6, 125)]
    [InlineData(4, -0.1)]
    [InlineData(5, -5)]
    [InlineData(6, -25)]
    public async Task SetToDoPercentCompleteAsync_WithInvalidData_ReturnsBadRequestResult(int id, double percentComplete)
    {
        var percentCompleteDto = new PercentCompleteDto
        {
            PercentComplete = percentComplete
        };

        var content = percentCompleteDto.ToJsonHttpContent();

        var response = await _client.PutAsync($"/api/todo/{id}/percent_complete", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task RemoveToDoAsync_WithCorrectId_ReturnsNoContentResult(int id)
    {
        var response = await _client.DeleteAsync($"/api/todo/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(102)]
    [InlineData(103)]
    public async Task RemoveToDoAsync_WithIncorrectId_ReturnsNotFoundResult(int id)
    {
        var response = await _client.DeleteAsync($"/api/todo/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task MarkToDoAsDoneAsync_WithCorrectId_ReturnsNoContentResult(int id)
    {
        var response = await _client.PutAsync($"/api/todo/{id}/done", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(102)]
    [InlineData(103)]
    public async Task MarkToDoAsDoneAsync_WithIncorrectId_ReturnsNotFoundResult(int id)
    {
        var response = await _client.PutAsync($"/api/todo/{id}/done", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
