namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class StudentProfile
{
    public int StudentProfileId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string? Introduction { get; set; }
    public string? CareerGoal { get; set; }
    public string? BusinessName { get; set; }
    public string? AboutBusiness { get; set; }
    public string? Offering { get; set; }
    public string? Owner { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<EducationHistory> EducationHistory { get; set; } = new List<EducationHistory>();
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public ContactInformation? ContactInformation { get; set; }
}
