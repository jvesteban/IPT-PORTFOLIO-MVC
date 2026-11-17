namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

// Project details displayed by the portfolio preview.
public sealed class StudentProjectViewModel
{
    public string? ImageUrl { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ProjectUrl { get; init; }
}
