// Ito ang counts at chart data na ipinapakita sa admin dashboard.
namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int TotalStudents { get; init; }
    public int ActiveStudents { get; init; }
    public int InactiveStudents { get; init; }
    public int NeverSignedIn { get; init; }
    // Iwasan ang division by zero kapag wala pang registered students.
    public double ActivePercent => TotalStudents == 0 ? 0 : 100d * ActiveStudents / TotalStudents;
    public IReadOnlyList<RegistrationMonthViewModel> Registrations { get; init; } = [];
    public IReadOnlyList<AdminStudentViewModel> RecentSignIns { get; init; } = [];
}
