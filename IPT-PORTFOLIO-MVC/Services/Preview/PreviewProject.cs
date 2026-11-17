namespace IPT_PORTFOLIO_MVC.Services.Preview;

// Temporary project data, stored with the student's other preview profile details.
public sealed record PreviewProject(string Title, string? Description, string? ProjectUrl)
{
    public string ProjectId { get; init; } = Guid.NewGuid().ToString("N");
    public string? ImageId { get; init; }
}
