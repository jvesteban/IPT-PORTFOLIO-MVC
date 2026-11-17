namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class Project
{
    public int ProjectId { get; set; }
    public int StudentProfileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ImageId { get; set; }
    public int DisplayOrder { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
