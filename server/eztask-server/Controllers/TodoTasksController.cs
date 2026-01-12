using Microsoft.AspNetCore.Mvc;
using EztaskServer.Todos;

namespace EztaskServer.Controllers;

[ApiController]
[Route("api/todos")]
public sealed class TodoTasksController : ControllerBase
{
    private readonly ITodoTaskService _service;

    public TodoTasksController(ITodoTaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoTaskDto>>> List(
        [FromQuery] bool? completed,
        CancellationToken ct)
        => Ok(await _service.List(completed, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoTaskDto>> Get(int id, CancellationToken ct)
    {
        var dto = await _service.Get(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TodoTaskDto>> Create(
        [FromBody] CreateTodoTaskRequest request,
        CancellationToken ct)
    {
        var dto = await _service.Create(request, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoTaskDto>> Update(
        int id,
        [FromBody] UpdateTodoTaskRequest request,
        CancellationToken ct)
    {
        var dto = await _service.Update(id, request, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("{id:int}/complete")]
    public async Task<ActionResult<TodoTaskDto>> Complete(int id, CancellationToken ct)
    {
        var dto = await _service.Complete(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.Delete(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
