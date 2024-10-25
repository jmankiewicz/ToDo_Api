using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;
using ToDo_Api.IntegrationTests.Helpers;
using ToDo_Api.Models;

namespace ToDo_Api.PerformanceTests;

/// <summary>
/// Class responsible for invoking <see cref="ToDo_Api"/> endpoints.
/// </summary>
[MemoryDiagnoser]
public class ToDoApiPerformanceTests
{
    private readonly HttpClient _client;

    /// <summary>
    /// Constructor responsible for creating client for HTTP requests.
    /// </summary>
    public ToDoApiPerformanceTests()
    {
        var factory = new WebApplicationFactory<Program>();

        _client = GetClient_InMemoryDatabase(factory);
        //_client = GetClient_RealDatabase(factory);
    }

    /// <summary>
    /// Creates client for HTTP requests.
    /// Finds real database service and switch it for InMemoryDatabase.
    /// </summary>
    /// <returns><see cref="HttpClient"/> built using InMemoryDatabase.</returns>
    private static HttpClient GetClient_InMemoryDatabase(WebApplicationFactory<Program> factory)
    {
        return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                        .SingleOrDefault(service
                            => service.ServiceType == typeof(DbContextOptions<ToDoDbContext>));

                    if (dbContextOptions is not null)
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

    /// <summary>
    /// Creates client for HTTP requests.
    /// </summary>
    /// <returns><see cref="HttpClient"/> built using real database.</returns>
    private static HttpClient GetClient_RealDatabase(WebApplicationFactory<Program> factory)
        => factory.CreateClient();

    [Benchmark]
    public async Task GetAllToDosTests()
    {
        await _client.GetAsync("/api/todo/all");
    }

    [Benchmark]
    public async Task GetToDoByIdTests()
    {
        await _client.GetAsync($"/api/todo/1");
    }

    [Benchmark]
    public async Task GetIncomingToDoTests_PeriodTimeToday()
    {
        await _client.GetAsync($"/api/todo/incoming/{PeriodTime.Today}");
    }

    [Benchmark]
    public async Task GetIncomingToDoTests_PeriodTimeNextDay()
    {
        await _client.GetAsync($"/api/todo/incoming/{PeriodTime.NextDay}");
    }

    [Benchmark]
    public async Task GetIncomingToDoTests_PeriodTimeCurrentWeek()
    {
        await _client.GetAsync($"/api/todo/incoming/{PeriodTime.CurrentWeek}");
    }

    [Benchmark]
    public async Task CreateToDoTests()
    {
        var createToDoDto = new CreateToDoDto
        {
            Title = "Title",
            Description = "Description",
            Expire = DateTime.Now.AddDays(1)
        };

        var httpContent = createToDoDto.ToJsonHttpContent();

        await _client.PostAsync("/api/todo", httpContent);
    }

    [Benchmark]
    public async Task UpdateToDoTests()
    {
        var updateToDoDto = new UpdateToDoDto
        {
            Title = "Updated title",
            Description = "Updated description",
            Expire = DateTime.Now.AddDays(2)
        };

        var httpContent = updateToDoDto.ToJsonHttpContent();

        await _client.PutAsync($"/api/todo/1", httpContent);
    }

    [Benchmark]
    public async Task SetToDoPercentCompleteTests()
    {
        var percentComplete = new PercentCompleteDto { PercentComplete = 50 };

        var httpContent = percentComplete.ToJsonHttpContent();

        await _client.PutAsync($"/api/todo/1/percent_complete", httpContent);
    }

    [Benchmark]
    public async Task RemoveToDoTests()
    {
        await _client.DeleteAsync($"/api/todo/1");
    }

    [Benchmark]
    public async Task MarkToDoAsDoneTests()
    {
        await _client.PutAsync($"/api/todo/1/done", null);
    }
}
