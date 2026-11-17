namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentEducationEditViewModel
{
    public string Level { get; set; } = string.Empty;
    public List<StudentEducationEntryEditViewModel> Entries { get; set; } = [];
}
