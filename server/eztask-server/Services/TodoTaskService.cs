namespace EztaskServer.Todos;

public sealed class TodoTaskService : ITodoTaskService
{
    private readonly AppDbContext _db;

    public TodoTaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TodoTaskDto> CreateAsync(CreateTodoTaskRequest request, CancellationToken ct = default)
    {
        var text = (request.Text ?? "").Trim();
        if (text.Length == 0) return;
        if (text.Length > 200)
            text = text.Substring(0, 200);

        var now = DateTimeOffset.UtcNow;

        var entity = new TodoTask
        {
            Title = text,

            IsCompleted = false,
            CreatedAtUtc = now.UtcDateTime,
        };

        _db.TodoTasks.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<TodoTaskDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.TodoTasks.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<TodoTaskDto>> ListAsync(bool? isCompleted = null, CancellationToken ct = default)
    {
        var q = _db.TodoTasks.AsNoTracking().AsQueryable();

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

    public async Task<TodoTaskDto?> UpdateAsync(int id, UpdateTodoTaskRequest request, CancellationToken ct = default)
    {
        var entity = await _db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.IsCompleted = request.Done; // Changed to only mutate IsCompleted from Done

        await _db.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task<TodoTaskDto?> CompleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        if (!entity.IsCompleted)
        {
            entity.IsCompleted = true;
            await _db.SaveChangesAsync(ct);
        }

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.TodoTasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        _db.TodoTasks.Remove(entity);
        await _db.SaveChangesAsync(ct);
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