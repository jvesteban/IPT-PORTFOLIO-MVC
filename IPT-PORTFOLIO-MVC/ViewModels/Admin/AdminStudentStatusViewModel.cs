using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class AdminStudentStatusViewModel
{
    [Required, StringLength(100)]
    public string StudentId { get; set; } = string.Empty;
    [Required]
    public bool? IsActive { get; set; }
    [StringLength(500)]
    public string? Reason { get; set; }
}
