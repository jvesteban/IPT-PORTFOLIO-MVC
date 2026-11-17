using IPT_PORTFOLIO_MVC.ViewModels.Student;

namespace IPT_PORTFOLIO_MVC.Services.Preview;

// Both portfolio views consume the same display model. An EF-backed reader can
// supply this model later without making the views depend on database entities.
public static class PreviewPortfolioMapper
{
    public static StudentPortfolioViewModel ToViewModel(
        PreviewStudentProfile profile, bool isOwnPortfolio, Func<string, string?> projectImageUrl) => new()
    {
        StudentId = profile.AccountId,
        IsOwnPortfolio = isOwnPortfolio,
        FirstName = profile.FirstName, MiddleName = profile.MiddleName, LastName = profile.LastName,
        BirthDate = profile.BirthDate, Email = profile.Email,
        Introduction = profile.Introduction, CareerGoal = profile.CareerGoal,
        Skills = profile.Skills, Hobbies = profile.Hobbies,
        BusinessName = profile.BusinessName, BusinessAbout = profile.BusinessAbout,
        Offerings = profile.Offerings, Owners = profile.Owners,
        ContactPhone = profile.ContactPhone, ContactAddress = profile.ContactAddress,
        Education = profile.Education,
        Projects = profile.Projects.Select(project => new StudentProjectViewModel
        {
            Title = project.Title, Description = project.Description, ProjectUrl = project.ProjectUrl,
            ImageUrl = project.ImageId is null ? null : projectImageUrl(project.ImageId)
        }).ToArray()
    };
}
