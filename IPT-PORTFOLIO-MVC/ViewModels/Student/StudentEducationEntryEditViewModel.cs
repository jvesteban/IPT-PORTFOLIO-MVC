// Ito ang school fields at validation ng isang grade o college year.
using System.ComponentModel.DataAnnotations;

namespace IPT_PORTFOLIO_MVC.ViewModels.Student;

public class StudentEducationEntryEditViewModel : IValidatableObject
{
    [Required, Display(Name = "Grade / year")]
    public string Grade { get; set; } = string.Empty;

    [StringLength(200), Display(Name = "School name")]
    public string? SchoolName { get; set; }

    [StringLength(200), Display(Name = "College course (optional)")]
    public string? CollegeCourse { get; set; }

    [StringLength(200), Display(Name = "Strand/Track (optional)")]
    public string? StrandTrack { get; set; }

    [Range(1900, 2100), Display(Name = "Start year")]
    public int? StartYear { get; set; }

    [Range(1900, 2100), Display(Name = "End year")]
    public int? EndYear { get; set; }

    // Alamin kung may input sa card para puwedeng manatiling blanko ang unused grade.
    public bool HasDetails => !string.IsNullOrWhiteSpace(SchoolName) || !string.IsNullOrWhiteSpace(CollegeCourse)
        || !string.IsNullOrWhiteSpace(StrandTrack) || StartYear.HasValue || EndYear.HasValue;

    // Kapag may input, kailangan ang school at parehong years na nasa tamang order.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!HasDetails) yield break;
        if (string.IsNullOrWhiteSpace(SchoolName))
            yield return new("Enter the school name.", [nameof(SchoolName)]);
        if (!StartYear.HasValue)
            yield return new("Enter the start year.", [nameof(StartYear)]);
        if (!EndYear.HasValue)
            yield return new("Enter the end year.", [nameof(EndYear)]);
        if (StartYear.HasValue && EndYear.HasValue && EndYear < StartYear)
            yield return new("End year must be the same as or later than start year.", [nameof(EndYear)]);
    }
}
