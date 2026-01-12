using Microsoft.EntityFrameworkCore;

namespace EztaskServer.Todos;

public sealed class TodoTaskService(AppDbContext db) : ITodoTaskService
{
    private const int MaxTitleLength = 500;

    public async Task<TodoTaskDto?> Create(CreateTodoTaskRequest request, CancellationToken ct = default)
    {
        var text = (request.Text ?? "").Trim();
        if (text.Length == 0) return null;
        text = text[..Math.Min(text.Length, MaxTitleLength)];

        var entity = new TodoTask
        {
            Title = text,
            IsCompleted = false,
            CreatedAtUtc = DateTime.UtcNow,
        };

        db.TodoTasks.Add(entity);
        await db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<TodoTaskDto?> Get(int id, CancellationToken ct = default)
    {
        var entity = await db.TodoTasks.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<TodoTaskDto>> List(bool? isCompleted = null, CancellationToken ct = default)
    {
        var q = db.TodoTasks.AsNoTracking();

        if (isCompleted is not null)
            q = q.Where(x => x.IsCompleted == isCompleted.Value);

        var items = await q
            .OrderBy(x => x.IsCompleted)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new TodoTaskDto(
                x.Id,
                x.Title,
                x.IsCompleted,
                x.CreatedAtUtc
            ))
            .ToListAsync(ct);

        return items;
    }

    public async Task<TodoTaskDto?> Update(int id, UpdateTodoTaskRequest request, CancellationToken ct = default)
    {
        var entity = await db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.IsCompleted = request.Done;

        await db.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task<TodoTaskDto?> Complete(int id, CancellationToken ct = default)
    {
        var entity = await db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        if (!entity.IsCompleted)
        {
            entity.IsCompleted = true;
            await db.SaveChangesAsync(ct);
        }

        return Map(entity);
    }

    public async Task<bool> Delete(int id, CancellationToken ct = default)
    {
        var entity = await db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.TodoTasks.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static TodoTaskDto Map(TodoTask x) =>
        new(
            x.Id,
            x.Title,
            x.IsCompleted,
            x.CreatedAtUtc
        );
}
