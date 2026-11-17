// Dito dumadaan ang student dashboard, portfolio preview, at pag-save ng edits.
using Microsoft.AspNetCore.Mvc;
using IPT_PORTFOLIO_MVC.ViewModels.Student;
using Microsoft.AspNetCore.Authorization;
using IPT_PORTFOLIO_MVC.Services.Preview;
using System.Security.Claims;
using IPT_PORTFOLIO_MVC.Services;

namespace IPT_PORTFOLIO_MVC.Controllers;

[Authorize(Policy = "Student")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class StudentController(AccountStore accounts, ProfilePhotoStore photos, ProjectImageStore projectImages, ILogger<StudentController> logger) : Controller
{
    // Sa login cookie kinukuha ang owner ID para sarili lang niyang profile ang ma-edit.
    private string CurrentAccountId => User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    [HttpGet]
    // Dalhin ang default student route sa dashboard.
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet]
    // Ipakita ang lahat ng student cards at unahin ang card ng naka-login na user.
    public IActionResult Dashboard()
    {
        var ownerId = CurrentAccountId;
        return View(new StudentDashboardViewModel
        {
            FirstName = accounts.GetProfile(ownerId)?.FirstName ?? "Student",
            Students = accounts.GetProfiles()
                .OrderByDescending(profile => profile.AccountId == ownerId)
                .ThenBy(profile => profile.LastName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(profile => profile.FirstName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(profile => profile.AccountId, StringComparer.Ordinal)
                .Select(profile => new StudentCardViewModel
                {
                    StudentId = profile.AccountId,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Introduction = string.IsNullOrWhiteSpace(profile.Introduction)
                        ? "This student is getting their portfolio started." : profile.Introduction,
                    IsOwnPortfolio = profile.AccountId == ownerId
                }).ToArray()
        });
    }

    [HttpGet]
    // Buksan ang napiling portfolio at alamin kung ang viewer ang owner nito.
    public IActionResult Portfolio(string? id)
    {
        var profile = accounts.GetProfile(id ?? CurrentAccountId);
        if (profile is null) return NotFound();

        return View(PreviewPortfolioMapper.ToViewModel(profile, profile.AccountId == CurrentAccountId,
            image => Url.Action("Show", "ProjectImages", new { id = profile.AccountId, image })));
    }

    [HttpGet]
    // Ilagay sa edit form ang kasalukuyang saved profile, projects, at education.
    public IActionResult Edit()
    {
        var profile = accounts.GetProfile(CurrentAccountId);
        if (profile is null) return NotFound();
        return View(new StudentProfileEditViewModel
        {
            StudentId = CurrentAccountId,
            FirstName = profile.FirstName, MiddleName = profile.MiddleName, LastName = profile.LastName,
            BirthDate = profile.BirthDate, Introduction = profile.Introduction, CareerGoal = profile.CareerGoal,
            Email = profile.Email, Skills = string.Join("\n\n", profile.Skills), Hobbies = string.Join("\n\n", profile.Hobbies),
            BusinessName = profile.BusinessName, BusinessAbout = profile.BusinessAbout,
            Offerings = string.Join("\n", profile.Offerings), Owners = string.Join("\n", profile.Owners),
            ContactPhone = profile.ContactPhone, ContactAddress = profile.ContactAddress,
            Projects = profile.Projects.Count == 0 ? [new()] : profile.Projects.Select(project => new StudentProjectEditViewModel
            {
                Title = project.Title, Description = project.Description, ProjectUrl = project.ProjectUrl,
                ProjectId = project.ProjectId,
                CurrentImageUrl = project.ImageId is null ? null : Url.Action("Show", "ProjectImages", new { id = profile.AccountId, image = project.ImageId })
            }).ToList(),
            // Gumawa ng fixed card kada grade kahit wala pang saved school details.
            Education = StudentProfileEditViewModel.EducationLevels.Select(level =>
            {
                var existing = profile.Education.FirstOrDefault(item => item.Level == level);
                return new StudentEducationEditViewModel
                {
                    Level = level,
                    Entries = PreviewEducation.Grades(level).Select(grade =>
                    {
                        var entries = existing?.Entries.Where(entry => entry.Grade == grade).ToArray() ?? [];
                        return new StudentEducationEntryEditViewModel
                        {
                            Grade = grade,
                            SchoolName = string.Join(" / ", entries.Select(entry => entry.SchoolName).Distinct()),
                            CollegeCourse = string.Join(" / ", entries.Select(entry => entry.CollegeCourse).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct()),
                            StrandTrack = string.Join(" / ", entries.Select(entry => entry.StrandTrack).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct()),
                            StartYear = entries.Select(entry => (int?)entry.StartYear).Min(),
                            EndYear = entries.Select(entry => (int?)entry.EndYear).Max()
                        };
                    }).ToList()
                };
            }).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    // I-check ang form at uploads bago i-save ang edits ng naka-login na student.
    public async Task<IActionResult> Edit(StudentProfileEditViewModel model, CancellationToken cancellationToken)
    {
        model.StudentId = CurrentAccountId;
        var existingProfile = accounts.GetProfile(CurrentAccountId);
        if (existingProfile is null) return NotFound();
        // I-check muna ang project images para may field error kapag invalid ang upload.
        var imageBytes = new Dictionary<StudentProjectEditViewModel, byte[]>();
        var newImages = new Dictionary<StudentProjectEditViewModel, string>();
        for (var i = 0; i < (model.Projects?.Count ?? 0); i++)
        {
            var project = model.Projects![i];
            if (project is null) continue;
            var existing = existingProfile.Projects.FirstOrDefault(item => item.ProjectId == project.ProjectId);
            if (existing?.ImageId is not null && !project.RemoveImage && string.IsNullOrWhiteSpace(project.Title))
                ModelState.AddModelError($"Projects[{i}].Title", "Enter a title for this project.");
            if (project.Image is null) continue;
            try
            {
                var checkedImage = await projectImages.ValidateAsync(project.Image, cancellationToken);
                if (checkedImage.Error is not null) ModelState.AddModelError($"Projects[{i}].Image", checkedImage.Error);
                else imageBytes[project] = checkedImage.Bytes!;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                logger.LogError(exception, "Could not read a project image.");
                ModelState.AddModelError($"Projects[{i}].Image", "Could not read this image. Choose it again.");
            }
        }
        // I-save lang ang bagong profile picture kapag pasado ang form validation.
        if (ModelState.IsValid && model.ProfilePicture is not null)
        {
            try
            {
                var error = await photos.SaveAsync(CurrentAccountId, model.ProfilePicture, cancellationToken);
                if (error is not null) ModelState.AddModelError(nameof(model.ProfilePicture), error);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                logger.LogError(exception, "Could not save the profile photo.");
                ModelState.AddModelError(nameof(model.ProfilePicture), "Photo could not be saved. Please try again.");
            }
        }
        if (ModelState.IsValid)
        {
            try
            {
                foreach (var item in imageBytes)
                    newImages[item.Key] = await projectImages.SaveAsync(CurrentAccountId, item.Value, cancellationToken);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or OperationCanceledException)
            {
                logger.LogError(exception, "Could not store project images.");
                foreach (var image in newImages.Values) DeleteUnusedProjectImage(image);
                newImages.Clear();
                ModelState.AddModelError("Projects", "Project images could not be saved. Please choose them again and retry.");
            }
        }
        // Ibalik ang inputs at image references sa form para maitama ang errors.
        if (!ModelState.IsValid)
        {
            model.Projects = (model.Projects ?? []).Where(project => project is not null).ToList();
            if (model.Projects.Count == 0) model.Projects.Add(new());
            for (var i = 0; i < model.Projects.Count; i++)
            {
                var project = model.Projects[i];
                var existing = existingProfile.Projects.FirstOrDefault(item => item.ProjectId == project.ProjectId);
                project.CurrentImageUrl = existing?.ImageId is null ? null :
                    Url.Action("Show", "ProjectImages", new { id = CurrentAccountId, image = existing.ImageId });
                if (project.Image is not null && !ModelState.ContainsKey($"Projects[{i}].Image"))
                    ModelState.AddModelError($"Projects[{i}].Image", "Choose this image again after correcting the other fields.");
            }
            model.Education = (model.Education ?? []).Take(4).Select(item => item ?? new()).ToList();
            while (model.Education.Count < 4) model.Education.Add(new());
            foreach (var level in model.Education)
                level.Entries = (level.Entries ?? []).Where(entry => entry is not null).ToList();
            return View(model);
        }

        // Linisin ang text at ayusin ang form data bago ipasa sa database service.
        var profile = new PreviewStudentProfile
        {
            FirstName = model.FirstName.Trim(), MiddleName = model.MiddleName?.Trim() ?? string.Empty,
            LastName = model.LastName.Trim(), BirthDate = model.BirthDate,
            Introduction = model.Introduction?.Trim() ?? string.Empty,
            CareerGoal = model.CareerGoal?.Trim() ?? string.Empty, Email = model.Email?.Trim() ?? string.Empty,
            BusinessName = model.BusinessName?.Trim() ?? string.Empty,
            BusinessAbout = model.BusinessAbout?.Trim() ?? string.Empty,
            Offerings = StudentProfileEditViewModel.SplitLines(model.Offerings),
            Owners = StudentProfileEditViewModel.SplitLines(model.Owners),
            ContactPhone = model.ContactPhone?.Trim() ?? string.Empty,
            ContactAddress = model.ContactAddress?.Trim() ?? string.Empty,
            Projects = (model.Projects ?? []).Where(project => project.HasDetails)
                .Select(project => new PreviewProject(project.Title!.Trim(),
                    project.Description?.Trim(), project.ProjectUrl?.Trim())
                {
                    ProjectId = existingProfile.Projects.FirstOrDefault(item => item.ProjectId == project.ProjectId)?.ProjectId ?? Guid.NewGuid().ToString("N"),
                    ImageId = newImages.GetValueOrDefault(project) ?? (project.RemoveImage ? null :
                        existingProfile.Projects.FirstOrDefault(item => item.ProjectId == project.ProjectId)?.ImageId)
                }).ToArray(),
            Skills = StudentProfileEditViewModel.SplitParagraphs(model.Skills),
            Hobbies = StudentProfileEditViewModel.SplitParagraphs(model.Hobbies),
            Education = model.Education.Select((item, index) => new PreviewEducation(
                    StudentProfileEditViewModel.EducationLevels[index],
                    item.Entries.Where(entry => entry.HasDetails)
                        .Select(entry => new PreviewEducationEntry(entry.Grade, entry.SchoolName!.Trim(),
                            entry.StartYear!.Value, entry.EndYear!.Value,
                            CollegeCourse: StudentProfileEditViewModel.EducationLevels[index] == "College" ? entry.CollegeCourse?.Trim() : null,
                            StrandTrack: StudentProfileEditViewModel.EducationLevels[index] == "Senior High" ? entry.StrandTrack?.Trim() : null)).ToArray()))
                .Where(level => level.Entries.Count > 0).ToArray()
        };
        if (!accounts.UpdateProfile(CurrentAccountId, profile))
        {
            foreach (var image in newImages.Values) DeleteUnusedProjectImage(image);
            return NotFound();
        }
        // Pagkatapos ma-save ang profile, alisin ang project images na hindi na ginagamit.
        var retainedImages = profile.Projects.Select(project => project.ImageId).ToHashSet();
        foreach (var image in existingProfile.Projects.Select(project => project.ImageId).OfType<string>().Where(image => !retainedImages.Contains(image)))
            DeleteUnusedProjectImage(image);
        TempData["ProfileSaved"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Portfolio));
    }
    // Subukang alisin ang lumang screenshot at i-log kung hindi ma-delete ang file.
    private void DeleteUnusedProjectImage(string imageId)
    {
        try { projectImages.Delete(CurrentAccountId, imageId); }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        { logger.LogWarning(exception, "Could not remove an unused project image."); }
    }
}
