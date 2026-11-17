// Ito ang personal at portfolio details na naka-link sa isang student account.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class StudentProfile
{
    // ID ng profile na may-ari ng mga details na ito.
    public int StudentProfileId { get; set; }
    // ID ng account na ginagamit sa pag-link ng mga record.
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
    // Optional photo reference sa profile record; ang current upload flow ay gumagamit ng account ID.
    public string? ProfilePhotoUrl { get; set; }
    // Date ng pinakahuling save ng details na ito.
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    // Related lists ito na binabasa at sine-save ng EF Core kasama ng profile.
    public ICollection<EducationHistory> EducationHistory { get; set; } = new List<EducationHistory>();
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    public ContactInformation? ContactInformation { get; set; }
}
