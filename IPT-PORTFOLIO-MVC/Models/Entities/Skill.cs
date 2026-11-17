namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class Skill
{
    public int SkillId { get; set; }
    public int StudentProfileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public StudentProfile StudentProfile { get; set; } = null!;
}
