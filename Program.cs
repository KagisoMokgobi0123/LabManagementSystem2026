
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. QUESTPDF CONFIGURATION
// ============================================================

QuestPDF.Settings.License = LicenseType.Community;


// ============================================================
// 2. DATABASE CONNECTION
// ============================================================

// ======================================================
// DATABASE CONFIGURATION
// Supports your existing "Con" connection string.
// Also supports "DefaultConnection" if configured later.
// ======================================================

var connectionString =
    builder.Configuration.GetConnectionString("Con")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'Con' or 'DefaultConnection' was not found.");
        "No connection string was found. Configure either " +
        "'Con' or 'DefaultConnection' in appsettings.json.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// ============================================================
// 3. ASP.NET CORE IDENTITY
// ============================================================

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
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(15);

    // User
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// ============================================================
// 4. EMAIL CONFIGURATION
// ============================================================

// Existing Identity email sender.
// ======================================================
// EMAIL SERVICES
// ======================================================

// ASP.NET Core Identity email service.
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Named technician SMTP configuration.
// Uses TechnicianEmailSettings from appsettings.json
// and User Secrets.
builder.Services.Configure<EmailSettings>(
    "Technician",
    builder.Configuration.GetSection("TechnicianEmailSettings"));


// ============================================================
// 5. AUTHENTICATION COOKIES
// ============================================================

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


// ============================================================
// 6. PASSWORD CHANGE FILTER
// ============================================================

builder.Services.AddScoped<
    NMB_HLabSys.Filters.RequirePasswordChangeFilter>();


// ============================================================
// 7. MVC CONTROLLERS AND VIEWS
// ============================================================

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<
        NMB_HLabSys.Filters.RequirePasswordChangeFilter>();
});


// ============================================================
// 8. DOCTOR SERVICES
// ============================================================

builder.Services.AddScoped<DoctorEmailService>();


// ============================================================
// 9. TECHNICIAN SERVICES
// ============================================================

builder.Services.AddScoped<TechnicianNotificationService>();


// ============================================================
// 10. LABORATORY EMAIL AND PDF SERVICES
// ============================================================

// Existing general email service.
builder.Services.AddScoped<EmailService>();

// Generates combined laboratory PDF reports.
builder.Services.AddScoped<LaboratoryPdfService>();

// Validates verified results and prepares PDF emails.
builder.Services.AddScoped<LaboratoryReportDeliveryService>();

// Sends laboratory reports through the technician SMTP account.
builder.Services.AddScoped<LaboratoryEmailService>();

builder.Services.AddHostedService<LaboratoryReportDispatcher>();





// ============================================================
// 11. CACHE AND SESSION
// ============================================================

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


// ============================================================
// 12. BUILD APPLICATION
// ============================================================

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


// ============================================================
// 13. ERROR HANDLING
// ============================================================

// ======================================================
// HTTP PIPELINE
// ======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// ============================================================
// 14. MIDDLEWARE
// ============================================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();


// ============================================================
// 15. AREA ROUTING
// ============================================================

// ======================================================
// ROUTES
// ======================================================

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// ============================================================
// 16. DEFAULT ROUTING
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ============================================================
// 17. IDENTITY ACCOUNT SEEDING
// ============================================================

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
        var logger =
            services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred while seeding Identity accounts.");
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(
            ex,
            "An error occurred while seeding Identity accounts.");
    }
}


// ============================================================
// 18. RUN APPLICATION
// ============================================================

app.Run();
// ======================================================
// START APPLICATION
// ======================================================

app.Run();