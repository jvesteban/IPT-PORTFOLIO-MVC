// Ito ang registration fields at rules para sa pangalan, username, at password.
using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Account;

public class RegisterViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Use between 8 and 128 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    // Dapat pareho ang password at confirmation bago payagan ang registration.
    [Compare(nameof(Password), ErrorMessage = "Your passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
