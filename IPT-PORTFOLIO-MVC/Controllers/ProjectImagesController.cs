// Dito ibinibigay ang screenshot na naka-link sa project ng student.
using IPT_PORTFOLIO_MVC.Services;
using IPT_PORTFOLIO_MVC.Services.Preview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPT_PORTFOLIO_MVC.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class ProjectImagesController(ProjectImageStore images, AccountStore accounts) : Controller
{
    [HttpGet]
    // I-check muna kung kabilang sa student project ang image bago ibigay ang file.
    public async Task<IActionResult> Show(string? id, string? image, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(image)
            || accounts.GetProfile(id)?.Projects.Any(project => project.ImageId == image) != true) return NotFound();
        var stored = await images.ReadAsync(id, image, cancellationToken);
        if (stored is null) return NotFound();
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(stored.Value.Bytes, stored.Value.ContentType);
    }
}
