// Dito kinukuha ang admin reports at pinapagana ang pag-manage ng student accounts.
using IPT_PORTFOLIO_MVC.Services;
using System.Globalization;
using IPT_PORTFOLIO_MVC.Services.Preview;
using IPT_PORTFOLIO_MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPT_PORTFOLIO_MVC.Controllers;

[Authorize(Policy = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AdminController(AccountStore accounts) : Controller
{
    [HttpGet]
    // Dalhin ang default admin route sa dashboard.
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet]
    // Kunin ang student counts, monthly registrations, at recent sign-ins.
    public IActionResult Dashboard()
    {
        var students = accounts.GetStudents();
        // Gamitin ang oras sa Pilipinas para tama ang pag-group ng registrations kada buwan.
        var localNow = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8));
        var thisMonth = new DateTime(localNow.Year, localNow.Month, 1);
        var months = Enumerable.Range(-5, 6).Select(offset => thisMonth.AddMonths(offset)).ToArray();
        var counts = months.Select(month => students.Count(student =>
        {
            var date = student.CreatedAt.ToOffset(TimeSpan.FromHours(8));
            return date.Year == month.Year && date.Month == month.Month;
        })).ToArray();
        var maximum = Math.Max(1, counts.Max());

        return View(new AdminDashboardViewModel
        {
            TotalStudents = students.Count,
            ActiveStudents = students.Count(student => student.IsActive),
            InactiveStudents = students.Count(student => !student.IsActive),
            NeverSignedIn = students.Count(student => student.LastLoginAt is null),
            Registrations = months.Select((month, index) => new RegistrationMonthViewModel
            {
                Label = month.ToString("MMM", CultureInfo.InvariantCulture),
                FullLabel = month.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                Count = counts[index], HeightPercent = 100d * counts[index] / maximum
            }).ToArray(),
            RecentSignIns = students.Where(student => student.LastLoginAt.HasValue)
                .OrderByDescending(student => student.LastLoginAt).Take(5).Select(ToStudent).ToArray()
        });
    }

    [HttpGet]
    // I-filter ang directory ayon sa pangalan, username, at account status.
    public IActionResult Students(string? search, string? status)
    {
        var students = accounts.GetStudents();
        var query = (search ?? string.Empty).Trim();
        var selectedStatus = status?.ToLowerInvariant() switch { "active" => "Active", "inactive" => "Inactive", _ => "All" };
        var matches = students.Select(ToStudent)
            .Where(student => selectedStatus == "All" || student.IsActive == (selectedStatus == "Active"))
            .Where(student => student.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) || student.Username.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(student => student.FullName, StringComparer.OrdinalIgnoreCase).ToArray();
        return View(new AdminStudentsViewModel { Search = query, Status = selectedStatus, TotalStudents = students.Count, Students = matches });
    }

    [HttpGet]
    // Kunin ang account at portfolio ng student na pinili ng admin.
    public IActionResult Student(string id)
    {
        var student = accounts.GetStudents().FirstOrDefault(item => item.AccountId == id);
        var profile = accounts.GetProfile(id);
        if (student is null || profile is null) return NotFound();
        return View(new AdminStudentDetailsViewModel
        {
            Student = ToStudent(student),
            Profile = PreviewPortfolioMapper.ToViewModel(profile, false,
                image => Url.Action("Show", "ProjectImages", new { id = profile.AccountId, image }))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // I-check ang reason bago baguhin ang status at magpakita ng feedback.
    public IActionResult SetStatus(AdminStudentStatusViewModel model)
    {
        if (!ModelState.IsValid || (model.IsActive == false && string.IsNullOrWhiteSpace(model.Reason)))
        {
            TempData["AdminNotice"] = "Choose a student and enter a reason of up to 500 characters when deactivating.";
            return RedirectToAction(nameof(Students));
        }
        var notice = accounts.SetStudentActive(model.StudentId, model.IsActive!.Value, model.Reason);
        if (notice is null) return NotFound();
        TempData["AdminNotice"] = notice;
        return RedirectToAction(nameof(Students));
    }

    // Gawing display data ang account summary at kunin ang pangalan sa profile.
    private AdminStudentViewModel ToStudent(PreviewStudentSummary account)
    {
        var profile = accounts.GetProfile(account.AccountId);
        return new AdminStudentViewModel
        {
            StudentId = account.AccountId,
            FullName = profile is null ? account.Username : $"{profile.FirstName} {profile.LastName}".Trim(),
            Username = account.Username, IsActive = account.IsActive,
            CreatedAt = account.CreatedAt, LastLoginAt = account.LastLoginAt
        };
    }
}
