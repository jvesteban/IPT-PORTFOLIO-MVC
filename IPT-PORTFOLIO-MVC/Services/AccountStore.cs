// Dito binabasa at sine-save ang accounts at portfolio data gamit ang EF Core.
using IPT_PORTFOLIO_MVC.Data;
using IPT_PORTFOLIO_MVC.Models.Entities;
using IPT_PORTFOLIO_MVC.Services.Preview;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IPT_PORTFOLIO_MVC.Services;

public sealed class AccountStore(ApplicationDbContext db)
{
    private readonly PasswordHasher<User> hasher = new();

    // Gumawa ng student account at kasamang profile kapag available ang username.
    public bool TryRegister(string username, string firstName, string lastName, string password)
    {
        username = username.Trim();
        firstName = firstName.Trim();
        lastName = lastName.Trim();
        if (db.Users.Any(u => u.Username == username)) return false;
        var user = new User
        {
            Username = username, FullName = $"{firstName} {lastName}", UserRole = "Student",
            StudentProfile = new StudentProfile { FirstName = firstName, LastName = lastName }
        };
        // Hash ang sine-save para ma-check ang password nang hindi iniimbak ang mismong text nito.
        user.PasswordHash = hasher.HashPassword(user, password);
        db.Users.Add(user);
        try { db.SaveChanges(); }
        // Hawakan ang duplicate username kahit sabay dumating ang registration requests.
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return false;
        }
        return true;
    }

    // I-check ang password, account status, at role bago payagan ang login.
    public User? Authenticate(string username, string password, out bool deactivated)
    {
        deactivated = false;
        var user = db.Users.SingleOrDefault(u => u.Username == username.Trim());
        if (user is null) return null;
        PasswordVerificationResult result;
        try { result = hasher.VerifyHashedPassword(user, user.PasswordHash, password); }
        catch (FormatException) { return null; }
        if (result == PasswordVerificationResult.Failed) return null;
        if (!user.IsActive)
        {
            deactivated = true;
            return null;
        }
        // Gawing eksaktong Admin o Student ang role para tumugma sa access rules ng app.
        var role = user.UserRole.Trim();
        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) user.UserRole = "Admin";
        else if (role.Equals("Student", StringComparison.OrdinalIgnoreCase)) user.UserRole = "Student";
        else return null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = hasher.HashPassword(user, password);
        user.LastLoginAt = DateTime.UtcNow;
        db.SaveChanges();
        return user;
    }

    // Hanapin ang account sa database gamit ang numeric account ID.
    public User? FindById(string? accountId) => int.TryParse(accountId, out var id)
        ? db.Users.AsNoTracking().SingleOrDefault(u => u.UserId == id) : null;

    // Kunin ang student account summaries para sa admin lists at reports.
    public IReadOnlyList<PreviewStudentSummary> GetStudents() =>
        db.Users.AsNoTracking().Where(u => u.UserRole == "Student")
            .Select(u => new { u.UserId, u.Username, u.IsActive, u.CreatedAt, u.LastLoginAt }).AsEnumerable()
            .Select(u => new PreviewStudentSummary(u.UserId.ToString(), u.Username, u.IsActive,
                DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc),
                u.LastLoginAt is DateTime time ? DateTime.SpecifyKind(time, DateTimeKind.Utc) : null)).ToArray();

    // I-save sa isang transaction ang status change at pending email nito.
    public string? SetStudentActive(string accountId, bool isActive, string? reason)
    {
        if (!int.TryParse(accountId, out var id)) return null;
        using var transaction = db.Database.BeginTransaction();
        var student = db.Users.Where(u => u.UserId == id && u.UserRole == "Student");
        if (student.Where(u => u.IsActive != isActive)
            .ExecuteUpdate(s => s.SetProperty(u => u.IsActive, isActive)) == 0)
            return student.Any() ? "The account already has this status." : null;

        string notice;
        // Kapag na-reactivate, alisin ang pending deactivation notice ng student.
        if (isActive)
        {
            db.PendingDeactivationEmails.Where(e => e.UserId == id).ExecuteDelete();
            notice = "Student account activated.";
        }
        else
        {
            var email = db.ContactInformation.Where(c => c.StudentProfile.UserId == id)
                .Select(c => c.Email).SingleOrDefault()?.Trim();
            if (System.Net.Mail.MailAddress.TryCreate(email, out var address) && address.Address == email)
            {
                // Sa email ng profile ipapadala ang reason ng deactivation.
                db.PendingDeactivationEmails.Add(new PendingDeactivationEmail
                {
                    UserId = id, Recipient = email, Reason = reason!.Trim(), NextAttemptAt = DateTime.UtcNow
                });
                db.SaveChanges();
                notice = $"Student account deactivated. Notification queued for {email}.";
            }
            else notice = "Student account deactivated. No email could be queued because the student has no valid profile email.";
        }
        transaction.Commit();
        return notice;
    }

    // Kunin ang basic profile details na kailangan ng dashboard cards.
    public IReadOnlyList<PreviewStudentProfile> GetProfiles() => db.StudentProfiles.AsNoTracking()
        .Where(p => p.User.UserRole == "Student").Select(p => new PreviewStudentProfile
        {
            AccountId = p.UserId.ToString(), FirstName = p.FirstName, LastName = p.LastName,
            Introduction = p.Introduction ?? ""
        }).ToArray();

    // Isama sa profile query ang contact, interests, projects, at education records.
    private IQueryable<StudentProfile> Profiles => db.StudentProfiles.Where(p => p.User.UserRole == "Student")
        .Include(p => p.ContactInformation).Include(p => p.Skills).Include(p => p.Hobbies)
        .Include(p => p.Projects).Include(p => p.EducationHistory).AsSplitQuery();

    // Buuin ang portfolio details mula sa profile at mga naka-link na tables.
    public PreviewStudentProfile? GetProfile(string? accountId)
    {
        if (!int.TryParse(accountId, out var id)) return null;
        var p = Profiles.AsNoTracking().SingleOrDefault(p => p.UserId == id);
        if (p is null) return null;
        return new PreviewStudentProfile
        {
            AccountId = p.UserId.ToString(), FirstName = p.FirstName, LastName = p.LastName,
            MiddleName = p.MiddleName ?? "", BirthDate = p.BirthDate, Introduction = p.Introduction ?? "",
            CareerGoal = p.CareerGoal ?? "", BusinessName = p.BusinessName ?? "", BusinessAbout = p.AboutBusiness ?? "",
            Offerings = Lines(p.Offering), Owners = Lines(p.Owner), Email = p.ContactInformation?.Email ?? "",
            ContactPhone = p.ContactInformation?.Phone ?? "", ContactAddress = p.ContactInformation?.Address ?? "",
            Skills = p.Skills.OrderBy(x => x.DisplayOrder).Select(x => x.Name).ToArray(),
            Hobbies = p.Hobbies.OrderBy(x => x.DisplayOrder).Select(x => x.Name).ToArray(),
            Projects = p.Projects.OrderBy(x => x.DisplayOrder).Select(x => new PreviewProject(x.Title, x.Description, x.ProjectUrl)
                { ProjectId = x.ProjectId.ToString(), ImageId = x.ImageId }).ToArray(),
            Education = p.EducationHistory.OrderBy(e => e.DisplayOrder).GroupBy(e => e.EducationLevel)
                .Select(g => new PreviewEducation(g.Key, g.Where(e => e.StartYear.HasValue && e.EndYear.HasValue)
                    .Select(e => new PreviewEducationEntry(e.Grade, e.SchoolName, e.StartYear!.Value, e.EndYear!.Value,
                        e.CollegeCourse, e.StrandTrack)).ToArray())).ToArray()
        };
    }

    // Hatiin ang saved text sa magkakahiwalay na non-empty lines.
    private static string[] Lines(string? value) => (value ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // I-update ang personal details at mga naka-link na portfolio records.
    public bool UpdateProfile(string accountId, PreviewStudentProfile input)
    {
        if (!int.TryParse(accountId, out var id)) return false;
        var p = Profiles.Include(p => p.User).SingleOrDefault(p => p.UserId == id);
        if (p is null) return false;
        p.FirstName = input.FirstName;
        p.LastName = input.LastName;
        p.User.FullName = $"{input.FirstName} {input.LastName}";
        p.MiddleName = input.MiddleName;
        p.BirthDate = input.BirthDate;
        p.Introduction = input.Introduction;
        p.CareerGoal = input.CareerGoal;
        p.BusinessName = input.BusinessName;
        p.AboutBusiness = input.BusinessAbout;
        p.Offering = string.Join("\n", input.Offerings);
        p.Owner = string.Join("\n", input.Owners);
        p.UpdatedAt = DateTime.UtcNow;
        // Gumawa ng contact record kung first time pa lang maglalagay ng contact details.
        p.ContactInformation ??= new ContactInformation();
        p.ContactInformation.Email = input.Email;
        p.ContactInformation.Phone = input.ContactPhone;
        p.ContactInformation.Address = input.ContactAddress;
        p.ContactInformation.UpdatedAt = p.UpdatedAt;
        p.Skills.Clear();
        foreach (var name in input.Skills) p.Skills.Add(new Skill { Name = name, DisplayOrder = p.Skills.Count });
        p.Hobbies.Clear();
        foreach (var name in input.Hobbies) p.Hobbies.Add(new Hobby { Name = name, DisplayOrder = p.Hobbies.Count });
        // Panatilihin ang existing project IDs at alisin ang projects na wala na sa submitted list.
        var oldProjects = p.Projects.ToDictionary(x => x.ProjectId.ToString());
        var order = 0;
        foreach (var item in input.Projects)
        {
            if (!oldProjects.Remove(item.ProjectId, out var project))
            {
                project = new Project();
                p.Projects.Add(project);
            }
            project.Title = item.Title;
            project.Description = item.Description;
            project.ImageId = item.ImageId;
            project.ProjectUrl = item.ProjectUrl;
            project.DisplayOrder = order++;
        }
        db.Projects.RemoveRange(oldProjects.Values);
        // Itugma ang school record sa level at grade para isang record lang kada fixed card.
        var oldEducation = p.EducationHistory.ToDictionary(e => (e.EducationLevel, e.Grade));
        order = 0;
        foreach (var level in input.Education)
        foreach (var item in level.Entries)
        {
            if (!oldEducation.Remove((level.Level, item.Grade), out var record))
            {
                record = new EducationHistory { EducationLevel = level.Level, Grade = item.Grade };
                p.EducationHistory.Add(record);
            }
            record.SchoolName = item.SchoolName;
            record.StartYear = checked((short)item.StartYear);
            record.EndYear = checked((short)item.EndYear);
            record.CollegeCourse = item.CollegeCourse;
            record.StrandTrack = item.StrandTrack;
            record.DisplayOrder = order++;
        }
        db.EducationHistory.RemoveRange(oldEducation.Values);
        db.SaveChanges();
        return true;
    }
}
