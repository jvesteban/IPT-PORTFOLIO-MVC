// Ito ang isang grade o college year na may school at start at end year.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class EducationHistory
{
    public int EducationHistoryId { get; set; }
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string? StrandTrack { get; set; }
    public String? CollegeCourse { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public short? StartYear { get; set; }
    public short? EndYear { get; set; }
    // Ito ang order ng item kapag ipinakita sa portfolio.
    public int DisplayOrder { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile StudentProfile { get; set; } = null!;
}
