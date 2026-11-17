// Ito ang email, phone, address, at website na naka-link sa student profile.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class ContactInformation
{
    public int ContactInformationId { get; set; }
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    // Date ng pinakahuling save ng details na ito.
    public DateTime UpdatedAt { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile StudentProfile { get; set; } = null!;
}
