
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure QuestPDF once at application startup.
// Use Community only if your organization meets QuestPDF's license eligibility.
QuestPDF.Settings.License = LicenseType.Community;

// ======================================================
// DATABASE CONFIGURATION
// Supports your existing "Con" connection string.
// Also supports "DefaultConnection" if configured later.
// ======================================================

var connectionString =
    builder.Configuration.GetConnectionString("Con")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No connection string was found. Configure either " +
        "'Con' or 'DefaultConnection' in appsettings.json.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ======================================================
// ASP.NET CORE IDENTITY CONFIGURATION
// ======================================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Sign in
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    // Password requirements
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Lockout
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    // User
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ======================================================
// EMAIL SERVICES
// ======================================================

// ASP.NET Core Identity email service.
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Doctor subsystem email service.
// Used for patient credentials and other doctor notifications.
builder.Services.AddScoped<DoctorEmailService>();

// ======================================================
// AUTHENTICATION COOKIE CONFIGURATION
// ======================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.Cookie.Name = "NMB_HLabSys.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// ======================================================
// MVC, CACHE AND SESSION
// ======================================================

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ======================================================
// LOGGING
// ASP.NET Core already configures logging from appsettings.json.
// This keeps the default logging providers enabled.
// ======================================================

builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ======================================================
// BUILD APPLICATION
// ======================================================

var app = builder.Build();

// ======================================================
// HTTP PIPELINE
// ======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// ======================================================
// ROUTES
// ======================================================

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ======================================================
// SEED IDENTITY ACCOUNTS
// ======================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        await SeedAccount.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred while seeding Identity accounts.");
    }
}

// ======================================================
// START APPLICATION
// ======================================================

app.Run();