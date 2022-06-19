using IdentityApp.Models;
using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Start add EntityFramework Core
var connectionString = builder.Configuration.GetConnectionString("AppDataConnection");
builder.Services.AddDbContext<ProductDbContext>(opts =>
{
    opts.UseSqlServer(connectionString)
    .EnableSensitiveDataLogging();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// End add EntityFramework Core

// Start add EntityFramework Core For Identity
builder.Services.AddDbContext<IdentityDbContext>(opts =>
{
    opts.UseSqlServer(connectionString, 
        opts => opts.MigrationsAssembly("IdentityApp"))
    .EnableSensitiveDataLogging();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// End add EntityFramework Core For Identity

builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

// Start add Identity
//builder.Services.AddDefaultIdentity<IdentityUser>(opts =>
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireNonAlphanumeric = false;
    opts.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();
// End add Identity

builder.Services.Configure<SecurityStampValidatorOptions>(opts =>
{
    opts.ValidationInterval = TimeSpan.FromMinutes(1);
});

builder.Services.AddScoped<TokenUrlEncoderService>();
builder.Services.AddScoped<IdentityEmailService>();

builder.Services.AddAuthentication()
    .AddGoogle(opts =>
    {
        opts.ClientId = builder.Configuration["Google:ClientId"];
        opts.ClientSecret = builder.Configuration["Google:ClientSecret"];
    });

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/SignIn";
    opts.LogoutPath = "/Identity/SignOut";
    opts.AccessDeniedPath = "/Identity/Forbidden";
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});

app.Run();
