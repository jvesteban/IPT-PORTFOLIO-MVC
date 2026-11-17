// Ito ang isang hobby entry na naka-link sa student profile.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class Hobby
{
    public int HobbyId { get; set; }
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    // Ito ang order ng item kapag ipinakita sa portfolio.
    public int DisplayOrder { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile StudentProfile { get; set; } = null!;
}
