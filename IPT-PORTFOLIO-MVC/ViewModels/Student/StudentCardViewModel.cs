// Ito ang pangalan, introduction, at owner flag para sa isang dashboard card.
namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentCardViewModel
{
    public string StudentId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Introduction { get; init; } = string.Empty;
    public bool IsOwnPortfolio { get; init; }
    // Buuin ang pangalan para sa card heading.
    public string FullName => $"{FirstName} {LastName}".Trim();
    // Gamitin ang unang letters bilang avatar kapag walang profile picture.
    public string Initials => string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Take(2).Select(part => part[0])).ToUpperInvariant();
}
