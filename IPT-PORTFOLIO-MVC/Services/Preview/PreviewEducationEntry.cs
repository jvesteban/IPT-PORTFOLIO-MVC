// Ito ang school at year details ng isang grade sa portfolio.
namespace IPT_PORTFOLIO_MVC.Services.Preview;

public sealed record PreviewEducationEntry(string Grade, string SchoolName, int StartYear, int EndYear,
    string? CollegeCourse = null, string? StrandTrack = null);
