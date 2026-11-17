namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentDashboardViewModel
{
    public string FirstName { get; init; } = string.Empty;
    public IReadOnlyList<StudentCardViewModel> Students { get; init; } = [];
}
