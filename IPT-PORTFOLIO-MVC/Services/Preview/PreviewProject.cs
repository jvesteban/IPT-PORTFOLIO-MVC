// Ito ang project details na ipinapasa mula sa database reader papunta sa portfolio.
namespace IPT_PORTFOLIO_MVC.Services.Preview;

// Ginagamit ang project details na ito sa pag-map ng saved profile papunta sa views.
public sealed record PreviewProject(string Title, string? Description, string? ProjectUrl)
{
    // Temporary ID muna ang bagong project; database ID ang ginagamit kapag saved na.
    public string ProjectId { get; init; } = Guid.NewGuid().ToString("N");
    public string? ImageId { get; init; }
}
