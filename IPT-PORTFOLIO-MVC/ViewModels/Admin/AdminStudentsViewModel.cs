namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class AdminStudentsViewModel
{
    public string Search { get; init; } = string.Empty;
    public string Status { get; init; } = "All";
    public int TotalStudents { get; init; }
    public IReadOnlyList<AdminStudentViewModel> Students { get; init; } = [];
}
