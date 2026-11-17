// Ito ang project record na may title, description, link, at optional image reference.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class Project
{
    public int ProjectId { get; set; }
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    // Reference ng screenshot file sa server storage.
    public string? ImageId { get; set; }
    // Ito ang order ng item kapag ipinakita sa portfolio.
    public int DisplayOrder { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile StudentProfile { get; set; } = null!;
}
