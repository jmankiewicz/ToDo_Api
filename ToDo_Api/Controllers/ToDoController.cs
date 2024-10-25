using Microsoft.AspNetCore.Mvc;
using ToDo_Api.Dtos;
using ToDo_Api.Entities;
using ToDo_Api.Models;
using ToDo_Api.Services;

namespace ToDo_Api.Controllers;

/// <summary>
/// Controller responsible for handling HTTP requests related to <see cref="ToDo"/> items.
/// Provides endpoints for creating, retrieving, updating, and deleting <see cref="ToDo"/> items.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ToDoController : ControllerBase
{
    private readonly ToDoService _toDoService;

    public ToDoController(ToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    /// <summary>
    /// Asynchronously retrieves all <see cref="ToDo"/> items from the service.
    /// </summary>
    /// <returns>
    /// An <see cref="ActionResult"/> containing a list of <see cref="ToDoDto"/> items.
    /// Returns a 200 Ok status on success.
    /// </returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ToDoDto>>> GetAllToDosAsync()
    {
        var toDosDto = await _toDoService.GetAllToDosAsync();

        return Ok(toDosDto);
    }

    /// <summary>
    /// Asynchronously retrieves <see cref="ToDo"/> item with specified ID.
    /// </summary>
    /// <param name="id">The ID of the ToDo item to retrieve.</param>
    /// <returns>
    /// An <see cref="ActionResult"/> containing a <see cref="ToDoDto"/> item.
    /// Returns a 200 Ok status on success.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ToDoDto>> GetToDoByIdAsync([FromRoute] int id)
    {
        var toDoDto = await _toDoService.GetToDoByIdAsync(id);

        return Ok(toDoDto);
    }

    /// <summary>
    /// Retrieves a list of <see cref="ToDo"/> items that are due within the specified <see cref="PeriodTime"/>.
    /// </summary>
    /// <param name="periodTime">
    /// An enum specifying the time period for filtering <see cref="ToDo"/> items based on their due date.
    /// </param>
    /// <returns>
    /// An <see cref="ActionResult"/> containing a list of <see cref="ToDoDto"/> items that match the specified <see cref="PeriodTime"/>.
    /// Returns a 200 Ok status on success.
    /// </returns>
    [HttpGet("incoming/{periodTime}")]
    public ActionResult<List<ToDoDto>> GetIncomingToDos([FromRoute] PeriodTime periodTime)
    {
        var toDosDto = _toDoService.GetIncomingToDos(periodTime);

        return Ok(toDosDto);
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="ToDo"/> item based on the provided data.
    /// </summary>
    /// <param name="createToDoDto">A DTO containing all necessary information to create a new <see cref="ToDo"/> item.</param>
    /// <returns>An <see cref="ActionResult"/> indicating that the <see cref="ToDo"/> item was successfully created.
    /// Returns a 201 Created status on success.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateToDoAsync([FromBody] CreateToDoDto createToDoDto)
    {
        var id = await _toDoService.CreateToDoAsync(createToDoDto);

        return Created($"/api/todo/{id}", null);
    }

    /// <summary>
    /// Asynchronously updates specified by <paramref name="id"/> <see cref="ToDo"/> item with provided data.
    /// </summary>
    /// <param name="updateToDoDto">A DTO containing all necessary information to update existing <see cref="ToDo"/> item.</param>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to update.</param>
    /// <returns>An <see cref="ActionResult"/> indicating that the <see cref="ToDo"/> item was successfully updated.
    /// Returns a 204 NoContent status on success.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateToDoAsync([FromBody] UpdateToDoDto updateToDoDto, [FromRoute] int id)
    {
        await _toDoService.UpdateToDoAsync(updateToDoDto, id);

        return NoContent();
    }

    /// <summary>
    /// Asynchronously updates the property <see cref="ToDo.CompletePercent"/> of specified by <paramref name="id"/> <see cref="ToDo"/> item.
    /// </summary>
    /// <param name="percentComplete">A DTO containing percent complete data.</param>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to update.</param>
    /// <returns>An <see cref="ActionResult"/> indicating that the <see cref="ToDo"/> item was successfully updated.
    /// Returns a 204 NoContent status on success.</returns>
    [HttpPut("{id}/percent_complete")]
    public async Task<ActionResult> SetToDoPercentCompleteAsync([FromBody] PercentCompleteDto percentComplete, [FromRoute] int id)
    {
        await _toDoService.SetToDoPercentCompleteAsync(percentComplete, id);

        return NoContent();
    }

    /// <summary>
    /// Asynchronously removes the specified by <paramref name="id"/> <see cref="ToDo"/> item.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to remove.</param>
    /// <returns>An <see cref="ActionResult"/> indicating that the <see cref="ToDo"/> item was successfully removed.
    /// Returns a 204 NoContent status on success.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveToDoAsync([FromRoute] int id)
    {
        await _toDoService.DeleteToDoAsync(id);

        return NoContent();
    }

    /// <summary>
    /// Asynchronously marks the specified <see cref="ToDo"/> item as done by setting its <see cref="ToDo.CompletePercent"/> to 100%.
    /// </summary>
    /// <param name="id">The ID of the <see cref="ToDo"/> item to update.</param>
    /// <returns>An <see cref="ActionResult"/> indicating that the <see cref="ToDo"/> item was successfully updated.
    /// Returns a 204 NoContent status on success.</returns>
    [HttpPut("{id}/done")]
    public async Task<ActionResult> MarkToDoAsDoneAsync([FromRoute] int id)
    {
        await _toDoService.MarkToDoAsDoneAsync(id);

        return NoContent();
    }
}
