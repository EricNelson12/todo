using EztaskServer.Todos;
using Microsoft.EntityFrameworkCore;

namespace eztask_server.Tests;

public class TodoTaskServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_WithValidText_CreatesAndReturnsTodoTask()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new TodoTaskService(db);
        var request = new CreateTodoTaskRequest("Buy groceries");

        // Act
        var result = await service.Create(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Buy groceries", result.Text);
        Assert.False(result.Done);
        Assert.True(result.Id > 0);

        // Verify it was persisted
        var savedTask = await db.TodoTasks.FirstOrDefaultAsync();
        Assert.NotNull(savedTask);
        Assert.Equal("Buy groceries", savedTask.Title);
    }

    [Fact]
    public async Task Create_WithLongText_TruncatesTo500Characters()
    {
        // Arrange
        using var db = CreateDbContext();
        var service = new TodoTaskService(db);
        var longText = new string('a', 600);
        var request = new CreateTodoTaskRequest(longText);

        // Act
        var result = await service.Create(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.Text.Length);
    }
}
