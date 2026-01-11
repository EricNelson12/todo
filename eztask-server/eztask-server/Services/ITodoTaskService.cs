namespace EztaskServer.Todos;

public interface ITodoTaskService
{
    Task<TodoTaskDto> CreateAsync(CreateTodoTaskRequest request, CancellationToken ct = default);
    Task<TodoTaskDto?> GetAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TodoTaskDto>> ListAsync(bool? isCompleted = null, CancellationToken ct = default);
    Task<TodoTaskDto?> UpdateAsync(int id, UpdateTodoTaskRequest request, CancellationToken ct = default);
    Task<TodoTaskDto?> CompleteAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}