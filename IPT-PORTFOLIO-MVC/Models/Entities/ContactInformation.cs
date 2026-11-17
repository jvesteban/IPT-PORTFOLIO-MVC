namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class ContactInformation
{
    public int ContactInformationId { get; set; }
    public int StudentProfileId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public DateTime UpdatedAt { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
