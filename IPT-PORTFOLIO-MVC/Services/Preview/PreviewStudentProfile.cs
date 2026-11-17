namespace IPT_PORTFOLIO_MVC.Services.Preview;

public sealed record PreviewStudentProfile
{
    public string AccountId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string MiddleName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly? BirthDate { get; init; }
    public string Introduction { get; init; } = string.Empty;
    public string CareerGoal { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string BusinessName { get; init; } = string.Empty;
    public string BusinessAbout { get; init; } = string.Empty;
    public IReadOnlyList<string> Offerings { get; init; } = [];
    public IReadOnlyList<string> Owners { get; init; } = [];
    public string ContactPhone { get; init; } = string.Empty;
    public string ContactAddress { get; init; } = string.Empty;
    public IReadOnlyList<PreviewEducation> Education { get; init; } = [];
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<string> Hobbies { get; init; } = [];
    public IReadOnlyList<PreviewProject> Projects { get; init; } = [];
}
