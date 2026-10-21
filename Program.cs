using admin_blazor.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using admin_blazor.Components;
using admin_blazor.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("LevelEnglish")
    ?? throw new InvalidOperationException(
        "Connection string 'LevelEnglish' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.MapGet("/db-check", async (IConfiguration config) =>
{
    var connectionString = config.GetConnectionString("LevelEnglish");

    await using var connection = new MySqlConnection(connectionString);
    await connection.OpenAsync();

    await using var command = new MySqlCommand(
        "SELECT COUNT(*) FROM users", connection);

    var count = await command.ExecuteScalarAsync();

    return Results.Ok(new { connected = true, users = count });
});

app.MapGet("/db-check-ef", async (AppDbContext db) =>
{
    var count = await db.Users.CountAsync();

    var users = await db.Users
        .AsNoTracking()
        .OrderBy(u => u.Id)
        .Take(5)
        .Select(u => new { u.Id, u.Name, u.Surname, u.UserType })
        .ToListAsync();

    return Results.Ok(new { count, users });
});



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
