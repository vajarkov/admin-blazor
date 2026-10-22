using System.Security.Claims;
using admin_blazor.Components;
using admin_blazor.Data;
using admin_blazor.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("LevelEnglish")
    ?? throw new InvalidOperationException(
        "Connection string 'LevelEnglish' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.Name = "LevelEnglish.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/auth/antiforgery", (
    HttpContext context,
    IAntiforgery antiforgery) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);

    return Results.Ok(new
    {
        token = tokens.RequestToken
    });
});

app.MapPost("/auth/login", async (
    HttpContext context,
    AuthService auth,
    IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(
            "Недействительный токен безопасности. Обновите страницу и попробуйте снова.");
    }

    var form = await context.Request.ReadFormAsync();

    var phone = form["Phone"].ToString();
    var password = form["Password"].ToString();

    if (string.IsNullOrWhiteSpace(phone) ||
        string.IsNullOrEmpty(password))
    {
        return Results.BadRequest("Введите телефон и пароль.");
    }

    var user = await auth.ValidateAdminAsync(phone, password);

    if (user is null)
    {
        return Results.Redirect("/login?error=1");
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Phone?.ToString("0") ?? ""),
        new Claim(
            "DisplayName",
            string.Join(" ", new[] { user.Name, user.Surname }
                .Where(s => !string.IsNullOrWhiteSpace(s)))),
        new Claim("Avatar", user.Avatar ?? ""),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var identity = new ClaimsIdentity(
        claims,
        CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    return Results.Redirect("/");
});


a
app.MapPost("/auth/logout", async (
    HttpContext context,
    IAntiforgery antiforgery) =>
{
    try
    {
        await antiforgery.ValidateRequestAsync(context);
    }
    catch (AntiforgeryValidationException)
    {
        return Results.BadRequest(
            "Недействительный токен безопасности. Обновите страницу и попробуйте снова.");
    }

    await context.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Redirect("/login");
});


app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
