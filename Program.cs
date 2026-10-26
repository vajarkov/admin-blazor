using System.Security.Claims;
using System.Net;
using admin_blazor.Components;
using admin_blazor.Data;
using admin_blazor.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("LevelEnglish")
    ?? throw new InvalidOperationException(
        "Connection string 'LevelEnglish' is not configured.");

// Постоянный каталог изображений.
// В production можно переопределить через Storage__UserImagesDirectory.
var userImagesDirectory =
    builder.Configuration["Storage:UserImagesDirectory"];

if (string.IsNullOrWhiteSpace(userImagesDirectory))
{
    userImagesDirectory = builder.Environment.IsDevelopment()
        ? Path.Combine(
            builder.Environment.ContentRootPath,
            "wwwroot",
            "images",
            "user")
        : "/var/lib/level-english-admin/images/user";
}

userImagesDirectory = Path.GetFullPath(userImagesDirectory);

builder.Configuration["Storage:UserImagesDirectory"] =
    userImagesDirectory;

Directory.CreateDirectory(userImagesDirectory);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

// Доверяем forwarded-заголовкам только от локального Apache.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownProxies.Clear();
    options.KnownNetworks.Clear();
    options.KnownProxies.Add(IPAddress.Loopback);
    options.ForwardLimit = 1;
});

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

// Обрабатываем заголовки прокси до HTTPS-редиректов и аутентификации.
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Обычные статические файлы приложения.
app.UseStaticFiles();

// Постоянные изображения доступны по прежним URL:
// /images/user/{filename}
// /images/user/teacher/{filename}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(userImagesDirectory),
    RequestPath = "/images/user"
});

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
