namespace IPT_PORTFOLIO_MVC.Services.Preview;

// Safe listing data; password hashes never leave the account store through this type.
public sealed record PreviewStudentSummary(
    string AccountId, string Username, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt);
