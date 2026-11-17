// Ito ang account summary para sa student list, kasama ang status at login dates.
namespace IPT_PORTFOLIO_MVC.Services.Preview;

// Pang-listahan lang ang fields dito para hindi maisama ang password hash sa view.
public sealed record PreviewStudentSummary(
    string AccountId, string Username, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset? LastLoginAt);
