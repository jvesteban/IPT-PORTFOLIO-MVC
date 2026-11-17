// Ito ang project details at image URL na ginagamit ng portfolio preview.
namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

// Title, description, link, at optional screenshot ang ipinapakita sa project card.
public sealed class StudentProjectViewModel
{
    public string? ImageUrl { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ProjectUrl { get; init; }
}
