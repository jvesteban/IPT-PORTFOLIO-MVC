// Ito ang social link record na naka-link sa student profile.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class SocialLink
{
    public int SocialLinkId { get; set; }
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    // Ito ang order ng item kapag ipinakita sa portfolio.
    public int DisplayOrder { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile StudentProfile { get; set; } = null!;
}
