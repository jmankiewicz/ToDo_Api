using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;
using ToDo_Api.Models;

namespace ToDo_Api.Services;

/// <summary>
/// Class responsible for <see cref="ToDoDbContext"/> operations.
/// </summary>
public class ToDoService
{
    private readonly ToDoDbContext _dbContext;
    private readonly IMapper _mapper;

    public ToDoService(ToDoDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Asynchronously retrieves all <see cref="ToDo"/> items from the database.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation. The <see cref="Task"/> result contains list of <see cref="ToDoDto"/> items.
    /// </returns>
    public async Task<List<ToDoDto>> GetAllToDosAsync()
    {
        var toDos = await _dbContext
            .ToDos
            .AsNoTracking()
            .ToListAsync();

        var toDosDto = _mapper.Map<List<ToDoDto>>(toDos);

        return toDosDto;
    }

    /// <summary>
    /// Asynchronously retrieves a <see cref="ToDo"/> item by its <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to retrieve.</param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation. The <see cref="Task"/> result contains <see cref="ToDoDto"/> item.
    /// </returns>
    /// <exception cref="BadHttpRequestException">
    /// Thrown if the <see cref="ToDo"/> item is not found.
    /// </exception>
    public async Task<ToDoDto> GetToDoByIdAsync(int id)
    {
        var toDo = await FirstToDoOrThrowExceptionAsync(id);

        var toDoDto = _mapper.Map<ToDoDto>(toDo);

        return toDoDto;
    }

    /// <summary>
    /// Retrieves a list of incoming <see cref="ToDo"/> items based on the specified <see cref="PeriodTime"/>.
    /// </summary>
    /// <param name="periodTime">The time period to filter the <see cref="ToDo"/> items.</param>
    /// <returns>
    /// A list of incoming <see cref="ToDoDto"/> items.
    /// </returns>
    public List<ToDoDto> GetIncomingToDos(PeriodTime periodTime)
    {
        var lastDay = LastDateForPeriod(periodTime);

        var toDos = _dbContext
            .ToDos
            .AsNoTracking()
            .Where(t => t.Expire <= lastDay.ToUniversalTime() &&
                    !t.IsDone &&
                    (periodTime == PeriodTime.NextDay ? t.Expire > AddTimeToToday(0, 23, 59, 59).ToUniversalTime()
                    : t.Expire > DateTime.Today.ToUniversalTime()));

        var toDosDto = _mapper.Map<List<ToDoDto>>(toDos);

        var orderedToDosDto = toDosDto
            .OrderBy(t => t.Expire)
            .ToList();

        return orderedToDosDto;
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="ToDo"/> item based on the provided <see cref="CreateToDoDto"/>.
    /// </summary>
    /// <param name="createToDoDto">The DTO containing data for the new <see cref="ToDo"/> item.</param>
    /// <returns>A <see cref="Task"/> containing ID of created <see cref="ToDo"/> item.</returns>
    public async Task<int> CreateToDoAsync(CreateToDoDto createToDoDto)
    {
        var toDo = _mapper.Map<ToDo>(createToDoDto);

        await _dbContext
            .ToDos
            .AddAsync(toDo);
        await _dbContext
            .SaveChangesAsync();

        return toDo.Id;
    }

    /// <summary>
    /// Asynchronously updates an existing <see cref="ToDo"/> item with the provided by <see cref="UpdateToDoDto"/> data.
    /// </summary>
    /// <param name="updateToDoDto">The DTO containing updated data for the <see cref="ToDo"/> item.</param>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to update.</param>
    public async Task UpdateToDoAsync(UpdateToDoDto updateToDoDto, int id)
    {
        var toDo = await FirstToDoOrThrowExceptionAsync(id);
        toDo.Title = updateToDoDto.Title;
        toDo.Description = updateToDoDto.Description;
        toDo.Expire = updateToDoDto.Expire;

        _dbContext
            .ToDos
            .Update(toDo);
        await _dbContext
            .SaveChangesAsync();
    }

    /// <summary>
    /// Asynchronously updates <see cref="ToDo.CompletePercent"/> parameter in <see cref="ToDo"/> item specified by <paramref name="id"/>.
    /// </summary>
    /// <param name="percentComplete">The DTO containing number of completed percent current <see cref="ToDo"/> item.</param>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to update.</param>
    public async Task SetToDoPercentCompleteAsync(PercentCompleteDto percentComplete, int id)
    {
        var toDo = await FirstToDoOrThrowExceptionAsync(id);

        toDo.CompletePercent = percentComplete.PercentComplete;
        toDo.IsDone = toDo.CompletePercent == 100;

        _dbContext
            .ToDos
            .Update(toDo);
        await _dbContext
            .SaveChangesAsync();
    }

    /// <summary>
    /// Asynchronously deletes a <see cref="ToDo"/> item by its <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to delete.</param>
    public async Task DeleteToDoAsync(int id)
    {
        var toDo = await FirstToDoOrThrowExceptionAsync(id);

        _dbContext
            .ToDos
            .Remove(toDo);
        await _dbContext
            .SaveChangesAsync();
    }

    /// <summary>
    /// Asynchronously marks the <see cref="ToDo"/> item as done by setting <see cref="ToDo.IsDone"/> to true and <see cref="ToDo.CompletePercent"/> to 100.
    /// Delegates the actual update to the <see cref="ToDoService.SetToDoPercentCompleteAsync(PercentCompleteDto, int)"/> method.
    /// </summary>
    /// <param name="id">The ID of <see cref="ToDo"/> item to update.</param>
    public async Task MarkToDoAsDoneAsync(int id)
    {
        await SetToDoPercentCompleteAsync(new PercentCompleteDto { PercentComplete = 100 }, id);
    }

    /// <summary>
    /// Adds the specified time to today's date.
    /// </summary>
    /// <returns>The <see cref="DateTime"/> object with fixed day and time.</returns>
    private static DateTime AddTimeToToday(int addDays, int addHours, int addMinutes, int addSeconds)
        => DateTime.Today
            .AddDays(addDays)
            .AddHours(addHours)
            .AddMinutes(addMinutes)
            .AddSeconds(addSeconds);

    /// <summary>
    /// Determines the last date for the specified <see cref="PeriodTime"/>.
    /// </summary>
    /// <param name="periodTime">An enum specifying the time period for filtering <see cref="ToDo"/> items based on their due date.</param>
    /// <returns>The <see cref="DateTime"/> object that represents the last date of given <see cref="PeriodTime"/>.</returns>
    private static DateTime LastDateForPeriod(PeriodTime periodTime)
        => periodTime switch
        {
            PeriodTime.Today => AddTimeToToday(0, 23, 59, 59),
            PeriodTime.NextDay => AddTimeToToday(1, 23, 59, 59),

            // For any other period, default to the last day of the current week.
            _ => LastDayOfCurrentWeek()
        };

    /// <summary>
    /// Determines the last day of current week.
    /// </summary>
    /// <returns>The <see cref="DateTime"/> object that represents the last day of current week.</returns>
    private static DateTime LastDayOfCurrentWeek()
    {
        var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)DateTime.Today.DayOfWeek + 7) % 7;

        return AddTimeToToday(daysUntilSunday, 23, 59, 59);
    }

    /// <summary>
    /// Asynchronously retrieves a <see cref="ToDo"/> item by its <paramref name="id"/> or throws an exception if not found.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to retrieve.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation. The <see cref="Task"/> result contains <see cref="ToDo"/> item.</returns>
    /// <exception cref="BadHttpRequestException">Thrown if the <see cref="ToDo"/> item is not found.</exception>
    private async Task<ToDo> FirstToDoOrThrowExceptionAsync(int id)
        => await _dbContext.ToDos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id)
           ?? throw new BadHttpRequestException($"ToDo with ID = {id} was not found.", 404);
}
