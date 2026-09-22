using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NMB_HLabSys.Data;
using NMB_HLabSys.Models.Account;
using NMB_HLabSys.Models.Identity;
using NMB_HLabSys.Services;
using NMB_HLabSys.Services.Implementations;
using NMB_HLabSys.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// DATABASE
// ============================================================

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("Con")));


// ============================================================
// MVC
// ============================================================

builder.Services.AddControllersWithViews();


// ============================================================
// HTTP CONTEXT
// ============================================================

builder.Services.AddHttpContextAccessor();


// ============================================================
// SESSION
// ============================================================
//
// DoctorDashboardController currently uses:
//
//     HttpContext.Session.GetString("UserId")
//
// Therefore ASP.NET Core Session must be registered.
//

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;
});


// ============================================================
// IDENTITY
// ============================================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ============================================================
// APPLICATION SERVICES
// ============================================================

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddScoped<ILabManagerService, LabManagerService>();

builder.Services.AddScoped<IReportService, ReportService>();

//builder.Services.AddScoped<IPasswordGeneratorService, PasswordGeneratorService>();

builder.Services.AddScoped<ILocationService, LocationService>();


// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// SEED ROLES / DEFAULT USERS
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    await RoleInitializer.InitializeAsync(roleManager);
}


// ============================================================
// HTTP REQUEST PIPELINE
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// ============================================================
// SESSION
// ============================================================
//
// IMPORTANT:
// UseSession() must be placed after UseRouting()
// and before the controllers execute.
//

app.UseSession();


// ============================================================
// AUTHENTICATION / AUTHORIZATION
// ============================================================

app.UseAuthentication();

app.UseAuthorization();


// ============================================================
// STATIC ASSETS
// ============================================================

app.MapStaticAssets();


// ============================================================
// AREA ROUTING
// ============================================================

app.MapControllerRoute(
    name: "areas",
    pattern:
        "{area:exists}/{controller=Home}/{action=Index}/{id?}");


// ============================================================
// DEFAULT ROUTING
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
