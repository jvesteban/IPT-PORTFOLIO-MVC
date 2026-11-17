namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentCardViewModel
{
    public string StudentId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Introduction { get; init; } = string.Empty;
    public bool IsOwnPortfolio { get; init; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Initials => string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Take(2).Select(part => part[0])).ToUpperInvariant();
}
