namespace EztaskServer.Todos;

public sealed record TodoTaskDto(
    int Id,
    string Title,
    bool IsCompleted,
    DateTime CreatedAtUtc
);

public sealed record CreateTodoTaskRequest(
    string Title,
    string? Notes,
    DateTimeOffset? DueAt
);

public sealed record UpdateTodoTaskRequest(
    string Title,
    string? Notes,
    DateTimeOffset? DueAt
);