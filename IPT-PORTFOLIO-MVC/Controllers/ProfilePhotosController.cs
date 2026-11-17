using IPT_PORTFOLIO_MVC.Services;
using IPT_PORTFOLIO_MVC.Services.Preview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPT_PORTFOLIO_MVC.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class ProfilePhotosController(ProfilePhotoStore photos, AccountStore accounts) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Show(string? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || accounts.GetProfile(id) is null) return NotFound();
        var photo = await photos.ReadAsync(id, cancellationToken);
        if (photo is null) return NotFound();
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(photo.Value.Bytes, photo.Value.ContentType);
    }
}
