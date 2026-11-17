namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class SocialLink
{
    public int SocialLinkId { get; set; }
    public int StudentProfileId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int DisplayOrder { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
