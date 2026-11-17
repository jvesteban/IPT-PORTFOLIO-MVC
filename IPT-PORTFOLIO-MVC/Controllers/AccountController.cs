// Dito dumadaan ang login, register, logout, at access denied na pages.
using IPT_PORTFOLIO_MVC.Services;
using Microsoft.AspNetCore.Mvc;
using IPT_PORTFOLIO_MVC.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using IPT_PORTFOLIO_MVC.Services.Preview;

namespace IPT_PORTFOLIO_MVC.Controllers;

public class AccountController(AccountStore accounts) : Controller
{
    [HttpGet]
    // Ipakita ang login form, o dalhin sa dashboard ang naka-login na account.
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Dashboard", User.IsInRole("Admin") ? "Admin" : "Student");

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // I-check ang login details at gumawa ng login cookie kapag valid ang account.
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var account = accounts.Authenticate(model.Username, model.Password, out var deactivated);
        if (account is null)
        {
            ModelState.AddModelError(string.Empty, deactivated
                ? "Your account has been deactivated. Contact your administrator to request reactivation."
                : "Invalid username or password.");
            return View(model);
        }

        // Ilagay sa login identity ang account ID, username, at role para sa access checks.
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.UserRole)
        ], "Student");

        await HttpContext.SignInAsync("Student", new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });

        return RedirectToAction("Dashboard", account.UserRole == "Admin" ? "Admin" : "Student");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // Alisin ang login cookie at bumalik sa login page.
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Student");
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    // Ipakita ang form para sa bagong student account.
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Dashboard", User.IsInRole("Admin") ? "Admin" : "Student");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // I-save ang bagong student kapag valid at available ang username.
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!accounts.TryRegister(model.Username, model.FirstName, model.LastName, model.Password))
        {
            ModelState.AddModelError(nameof(model.Username), "That username is already taken. Choose another one.");
            return View(model);
        }

        TempData["RegistrationSuccess"] = "Account created. Sign in with your username and password.";
        return RedirectToAction(nameof(Login));
    }

    // Ipakita ang 403 page kapag walang access ang role sa page.
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        ViewData["ErrorTitle"] = "Access denied";
        ViewData["ErrorMessage"] = "Your account does not have access to this page.";
        return View("Error");
    }

    // Ipakita ang error page na ginagamit ng app.
    public IActionResult Error() => View();
}
