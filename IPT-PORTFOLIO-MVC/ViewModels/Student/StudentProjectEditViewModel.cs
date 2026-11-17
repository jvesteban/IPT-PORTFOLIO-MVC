using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public sealed class StudentProjectEditViewModel : IValidatableObject
{
    public string? ProjectId { get; set; }
    [Display(Name = "Project image")]
    public IFormFile? Image { get; set; }
    public bool RemoveImage { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public string? CurrentImageUrl { get; set; }

    [StringLength(200), Display(Name = "Project title")]
    public string? Title { get; set; }

    [StringLength(10000), Display(Name = "Description")]
    public string? Description { get; set; }

    [StringLength(500), Display(Name = "Project link")]
    public string? ProjectUrl { get; set; }

    public bool HasDetails => !string.IsNullOrWhiteSpace(Title)
        || !string.IsNullOrWhiteSpace(Description) || !string.IsNullOrWhiteSpace(ProjectUrl) || Image is not null;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!HasDetails) yield break;
        if (string.IsNullOrWhiteSpace(Title))
            yield return new("Enter a title for this project.", [nameof(Title)]);
        if (!string.IsNullOrWhiteSpace(ProjectUrl)
            && (!Uri.TryCreate(ProjectUrl.Trim(), UriKind.Absolute, out var url)
                || (url.Scheme != Uri.UriSchemeHttps && url.Scheme != Uri.UriSchemeHttp)
                || string.IsNullOrWhiteSpace(url.Host)))
            yield return new("Use a full http:// or https:// project link, or leave it blank.", [nameof(ProjectUrl)]);
    }
}
