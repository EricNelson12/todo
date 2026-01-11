using System.ComponentModel.DataAnnotations;

namespace EztaskServer.Todos;

public sealed record TodoTaskDto(
    int Id,
    string Text,
    bool Done,
    DateTime CreatedAtUtc // Added CreatedAtUtc to match constructor usage
);

public sealed record CreateTodoTaskRequest(
    [Required] string Text
);

public sealed record UpdateTodoTaskRequest(
    bool Done
);