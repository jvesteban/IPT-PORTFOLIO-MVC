// Ito ang email na naghihintay ipadala matapos ma-deactivate ang student.
using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.Models.Entities;

public sealed class PendingDeactivationEmail
{
    [Key]
    // ID ng account na ginagamit sa pag-link ng mga record.
    public int UserId { get; set; }
    [MaxLength(256)]
    public string Recipient { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    // Susunod na oras kung kailan puwedeng subukang ipadala ang email.
    public DateTime NextAttemptAt { get; set; }
}
