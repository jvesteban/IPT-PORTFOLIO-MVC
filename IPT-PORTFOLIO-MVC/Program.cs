using IPT_PORTFOLIO_MVC.Services;
using IPT_PORTFOLIO_MVC.Data;
using Microsoft.EntityFrameworkCore;
using IPT_PORTFOLIO_MVC.Services.Preview;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AccountStore>();
builder.Services.AddHostedService<DeactivationEmailWorker>();
builder.Services.AddSingleton<IPT_PORTFOLIO_MVC.Services.ProfilePhotoStore>();
builder.Services.AddSingleton<IPT_PORTFOLIO_MVC.Services.ProjectImageStore>();
// Recheck account status and role for each authenticated request.
var applicationRun = Guid.NewGuid().ToString("N");
builder.Services.AddAuthentication("Student")
    .AddCookie("Student", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = ".IptPortfolio.Account";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Events.OnSigningIn = context =>
        {
            context.Properties.Items["ApplicationRun"] = applicationRun;
            return Task.CompletedTask;
        };
        options.Events.OnValidatePrincipal = async context =>
        {
            if (!context.Properties.Items.TryGetValue("ApplicationRun", out var run) || run != applicationRun)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync("Student");
                return;
            }
            var accounts = context.HttpContext.RequestServices.GetRequiredService<AccountStore>();
            var accountId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var account = accounts.FindById(accountId);
            if (account is null || !account.IsActive || account.UserRole != context.Principal?.FindFirst(ClaimTypes.Role)?.Value)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync("Student");
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Student", policy => policy
        .RequireAuthenticatedUser()
        .RequireRole("Student"));
    options.AddPolicy("Admin", policy => policy
        .RequireAuthenticatedUser()
        .RequireRole("Admin"));
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
