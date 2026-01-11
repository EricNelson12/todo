using Microsoft.EntityFrameworkCore;

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
        var title = (request.Title ?? "").Trim();
        if (title.Length == 0) throw new ArgumentException("Title is required.", nameof(request.Title));
        if (title.Length > 200) throw new ArgumentException("Title is too long (max 200).", nameof(request.Title));

        var now = DateTimeOffset.UtcNow;

        var entity = new TodoTask
        {
            Title = title,

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

        var title = (request.Title ?? "").Trim();
        if (title.Length == 0) throw new ArgumentException("Title is required.", nameof(request.Title));
        if (title.Length > 200) throw new ArgumentException("Title is too long (max 200).", nameof(request.Title));

        entity.Title = title;


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