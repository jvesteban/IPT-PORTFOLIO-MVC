// Pinagsasama nito ang account summary at portfolio para sa admin student view.
using IPT_PORTFOLIO_MVC.ViewModels.Student;

namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class AdminStudentDetailsViewModel
{
    public AdminStudentViewModel Student { get; init; } = new();
    public StudentPortfolioViewModel Profile { get; init; } = new();
}
