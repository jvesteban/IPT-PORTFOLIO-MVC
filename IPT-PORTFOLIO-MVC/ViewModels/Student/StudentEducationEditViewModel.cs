// Ito ang isang education level at ang fixed grade cards nito sa edit form.
namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentEducationEditViewModel
{
    public string Level { get; set; } = string.Empty;
    public List<StudentEducationEntryEditViewModel> Entries { get; set; } = [];
}
