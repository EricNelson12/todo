using Microsoft.EntityFrameworkCore;
using EztaskServer.Todos;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI and Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers and EF Core
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));
// Move the service registration above the app.Build() call
// Ensure the namespace is imported if it exists


builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();

var app = builder.Build();

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();