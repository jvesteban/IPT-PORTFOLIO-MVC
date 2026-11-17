using IPT_PORTFOLIO_MVC.Services.Preview;

namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentPortfolioViewModel
{
    public string StudentId { get; init; } = string.Empty;
    public bool IsOwnPortfolio { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string MiddleName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public string Introduction { get; init; } = string.Empty;
    public string CareerGoal { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public string BusinessAbout { get; init; } = string.Empty;
    public IReadOnlyList<string> Offerings { get; init; } = [];
    public IReadOnlyList<string> Owners { get; init; } = [];
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactAddress { get; init; } = string.Empty;
    public IReadOnlyList<PreviewEducation> Education { get; init; } = [];
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<string> Hobbies { get; init; } = [];
    public IReadOnlyList<StudentProjectViewModel> Projects { get; init; } = [];

    public string FullName => string.Join(" ",
        new[] { FirstName, MiddleName, LastName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));

    public string Initials
    {
        get
        {
            var first = !string.IsNullOrWhiteSpace(FirstName) ? FirstName[0].ToString() : string.Empty;
            var last = !string.IsNullOrWhiteSpace(LastName) ? LastName[0].ToString() : string.Empty;
            var combined = $"{first}{last}";
            return string.IsNullOrEmpty(combined) ? "ST" : combined.ToUpperInvariant();
        }
    }
}
