namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class EducationHistory
{
    public int EducationHistoryId { get; set; }
    public int StudentProfileId { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string? StrandTrack { get; set; }
    public String? CollegeCourse { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public short? StartYear { get; set; }
    public short? EndYear { get; set; }
    public int DisplayOrder { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
