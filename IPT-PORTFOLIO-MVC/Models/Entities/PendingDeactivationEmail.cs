using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.Models.Entities;

public sealed class PendingDeactivationEmail
{
    [Key]
    public int UserId { get; set; }
    [MaxLength(256)]
    public string Recipient { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    public DateTime NextAttemptAt { get; set; }
}
