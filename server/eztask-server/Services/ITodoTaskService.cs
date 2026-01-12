namespace EztaskServer.Todos;

public interface ITodoTaskService
{
    Task<TodoTaskDto?> Create(CreateTodoTaskRequest request, CancellationToken ct = default);
    Task<TodoTaskDto?> Get(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TodoTaskDto>> List(bool? isCompleted = null, CancellationToken ct = default);
    Task<TodoTaskDto?> Update(int id, UpdateTodoTaskRequest request, CancellationToken ct = default);
    Task<TodoTaskDto?> Complete(int id, CancellationToken ct = default);
    Task<bool> Delete(int id, CancellationToken ct = default);
}
