using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Areas.Doctor.Services;
using NMB_HLabSys.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. QuestPDF configuration
QuestPDF.Settings.License = LicenseType.Community;

// 2. Database configuration
var connectionString =
    builder.Configuration.GetConnectionString("Con")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'Con' or 'DefaultConnection' was not found in appsettings.json or configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 4. Email configuration
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<EmailSettings>(
    "Technician",
    builder.Configuration.GetSection("TechnicianEmailSettings"));

// 5. Doctor and technician services
builder.Services.AddScoped<DoctorEmailService>();
builder.Services.AddScoped<TechnicianNotificationService>();

// 6. Authentication cookies
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

// 7. Password change filter
builder.Services.AddScoped<
    NMB_HLabSys.Filters.RequirePasswordChangeFilter>();

// 8. MVC, cache and session
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<
        NMB_HLabSys.Filters.RequirePasswordChangeFilter>();
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 9. Laboratory email and PDF services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<LaboratoryPdfService>();
builder.Services.AddScoped<LaboratoryReportDeliveryService>();
builder.Services.AddScoped<LaboratoryEmailService>();

builder.Services.AddHostedService<LaboratoryReportDispatcher>();

// 10. Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 11. Build application
var app = builder.Build();

// 12. Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 13. Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 14. Area routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 15. Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 16. Seed Identity accounts
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

// 17. Start application
app.Run();
