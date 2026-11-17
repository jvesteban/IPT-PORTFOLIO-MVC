// Dito nagsisimula ang app at sine-set up ang services, login, database, at routes.
using IPT_PORTFOLIO_MVC.Services;
using IPT_PORTFOLIO_MVC.Data;
using Microsoft.EntityFrameworkCore;
using IPT_PORTFOLIO_MVC.Services.Preview;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// I-register ang MVC at services para magamit sila ng controllers at background worker.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AccountStore>();
builder.Services.AddHostedService<DeactivationEmailWorker>();
builder.Services.AddSingleton<IPT_PORTFOLIO_MVC.Services.ProfilePhotoStore>();
builder.Services.AddSingleton<IPT_PORTFOLIO_MVC.Services.ProjectImageStore>();
// Bawat request, i-check ulit kung active pa ang account at tama pa ang role.
// Bagong run ID kada start para bumalik sa login ang lumang session kapag ni-restart ang app.
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
        // Itabi sa login cookie kung saang run ng app ginawa ang session.
        options.Events.OnSigningIn = context =>
        {
            context.Properties.Items["ApplicationRun"] = applicationRun;
            return Task.CompletedTask;
        };
        // I-reject ang lumang session o account na na-deactivate o nagbago ang role.
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
// Admin at Student ang roles na kailangan para sa kani-kanilang pages.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Student", policy => policy
        .RequireAuthenticatedUser()
        .RequireRole("Student"));
    options.AddPolicy("Admin", policy => policy
        .RequireAuthenticatedUser()
        .RequireRole("Admin"));
});
// Dito ginagamit ang SQL Server connection string para sa EF Core.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ayusin ang order ng error handling, routing, login checks, at page routes.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Error");
    // Sa production, sabihan ang browser na gumamit ng HTTPS para sa site.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Basahin muna ang login cookie bago i-check ang access sa route.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Login page ang default na bubukas kapag walang controller at action sa URL.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
