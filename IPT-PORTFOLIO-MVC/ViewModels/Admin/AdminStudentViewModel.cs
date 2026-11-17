// Ito ang account details at date labels para sa student list ng admin.
using System.Globalization;

namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class AdminStudentViewModel
{
    public string StudentId { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? LastLoginAt { get; init; }
    // I-display sa oras ng Pilipinas ang registration at huling login date.
    public string RegisteredLabel => CreatedAt.ToOffset(TimeSpan.FromHours(8)).ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
    public string LastLoginLabel => LastLoginAt?.ToOffset(TimeSpan.FromHours(8)).ToString("MMM d, yyyy · h:mm tt", CultureInfo.InvariantCulture) ?? "Never signed in";
}
