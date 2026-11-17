using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Account;

public class LoginViewModel
{
    [Required, StringLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), StringLength(128)]
    public string Password { get; set; } = string.Empty;
}
