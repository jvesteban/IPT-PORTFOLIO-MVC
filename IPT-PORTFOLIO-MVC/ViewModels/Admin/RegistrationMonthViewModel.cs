// Ito ang label, bilang, at taas ng isang bar sa monthly registrations chart.
namespace IPT_PORTFOLIO_MVC.ViewModels.Admin;

public class RegistrationMonthViewModel
{
    public string Label { get; init; } = string.Empty;
    public string FullLabel { get; init; } = string.Empty;
    public int Count { get; init; }
    public double HeightPercent { get; init; }
}
