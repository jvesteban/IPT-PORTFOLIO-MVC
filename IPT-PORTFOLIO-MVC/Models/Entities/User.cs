// Ito ang account record sa Users table para sa Admin at Student.
namespace IPT_PORTFOLIO_MVC.Models.Entities;

public class User
{
    // ID ng account na ginagamit sa pag-link ng mga record.
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    // Hash ng password ang iniimbak at ginagamit sa login check.
    public string PasswordHash { get; set; } = string.Empty;
    // Dito nakabase kung Admin o Student ang access ng account.
    public string UserRole { get; set; } = "Student";
    // Account status ito na chine-check bago at habang naka-login.
    public bool IsActive { get; set; } = true;
    // Date ng registration na ginagamit sa reports.
    public DateTime CreatedAt { get; set; }
    // Huling successful login; blanko ito kung hindi pa nakapag-login.
    public DateTime? LastLoginAt { get; set; }

    // Link papunta sa profile; ginagamit ito ng EF Core para kunin ang related record.
    public StudentProfile? StudentProfile { get; set; }
}
