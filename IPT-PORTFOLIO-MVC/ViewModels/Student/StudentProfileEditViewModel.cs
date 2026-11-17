using IPT_PORTFOLIO_MVC.Services.Preview;
using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentProfileEditViewModel : IValidatableObject
{
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public string StudentId { get; set; } = string.Empty;

    [Display(Name = "Profile picture")]
    public IFormFile? ProfilePicture { get; set; }

    public static IReadOnlyList<string> EducationLevels => PreviewEducation.Levels;

    [Required, StringLength(100), Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(100), Display(Name = "Middle name")]
    public string? MiddleName { get; set; }
    [Required, StringLength(100), Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;
    [DataType(DataType.Date), Display(Name = "Birthday")]
    public DateOnly? BirthDate { get; set; }
    [StringLength(10000), Display(Name = "Personal introduction")]
    public string? Introduction { get; set; }
    [StringLength(10000), Display(Name = "Career goal")]
    public string? CareerGoal { get; set; }
    [EmailAddress, StringLength(256), Display(Name = "Email (optional)")]
    public string? Email { get; set; }
    [StringLength(100), Display(Name = "Business or company name")]
    public string? BusinessName { get; set; }
    [StringLength(10000), Display(Name = "About the business or company")]
    public string? BusinessAbout { get; set; }
    [StringLength(10000), Display(Name = "Products and services")]
    public string? Offerings { get; set; }
    [StringLength(2100), Display(Name = "Owners")]
    public string? Owners { get; set; }
    [StringLength(40), Display(Name = "Contact number")]
    public string? ContactPhone { get; set; }
    [StringLength(300), Display(Name = "Address or location")]
    public string? ContactAddress { get; set; }
    [StringLength(10000)]
    public string? Skills { get; set; }
    [StringLength(10000), Display(Name = "Passion and hobbies")]
    public string? Hobbies { get; set; }
    public List<StudentProjectEditViewModel> Projects { get; set; } = [new()];

    public List<StudentEducationEditViewModel> Education { get; set; } =
        EducationLevels.Select(level => new StudentEducationEditViewModel { Level = level }).ToList();

    public static string[] SplitParagraphs(string? value) => (value ?? string.Empty)
        .ReplaceLineEndings("\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    public static string[] SplitLines(string? value) => (value ?? string.Empty)
        .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToArray();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Projects is null || Projects.Any(project => project is null))
            yield return new("Use the project cards provided.", [nameof(Projects)]);
        if (BirthDate is { } date && (date.Year < 1900 || date > DateOnly.FromDateTime(DateTime.Today)))
            yield return new("Enter a birthday between 1900 and today.", [nameof(BirthDate)]);
        if (Education?.Count != 4 || Education.Any(item => item is null))
            yield return new("Use the four education sections provided.", [nameof(Education)]);
        else
        {
            for (var i = 0; i < Education.Count; i++)
            {
                var grades = PreviewEducation.Grades(EducationLevels[i]);
                if (Education[i].Level != EducationLevels[i])
                    yield return new("Use the education levels provided.", [$"Education[{i}].Level"]);
                if (Education[i].Entries is null || !Education[i].Entries.Select(entry => entry?.Grade).SequenceEqual(grades))
                    yield return new("Select a valid grade or year for this education level.", [$"Education[{i}].Entries"]);
            }
        }
        var owners = SplitLines(Owners);
        if (owners.Length > 20 || owners.Any(owner => owner.Length > 100))
            yield return new("Owners: use up to 20 lines, with no more than 100 characters per line.", [nameof(Owners)]);
    }
}
