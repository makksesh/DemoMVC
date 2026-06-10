using DemoMVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[] { System.Globalization.CultureInfo.InvariantCulture };

// === Подключение контекста базы данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// === Настройка аутентификации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// === Настройка авторизации (+ политики)
builder.Services.AddAuthorization(options =>
{
    var policiesSection = builder.Configuration.GetSection("Policies");
    foreach (var policy in policiesSection.GetChildren())
    {
        var roles = policy.GetSection("Roles").Get<string[]>() ?? Array.Empty<string>();
        var policyName = policy.Key;
        options.AddPolicy(policyName, p => p.RequireRole(roles));
    }
});


builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(
        System.Globalization.CultureInfo.InvariantCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
