using IPT_PORTFOLIO_MVC.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPT_PORTFOLIO_MVC.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<PendingDeactivationEmail> PendingDeactivationEmails => Set<PendingDeactivationEmail>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<EducationHistory> EducationHistory => Set<EducationHistory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Hobby> Hobbies => Set<Hobby>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();
    public DbSet<ContactInformation> ContactInformation => Set<ContactInformation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PendingDeactivationEmail>()
            .HasOne<User>().WithOne().HasForeignKey<PendingDeactivationEmail>(email => email.UserId);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", table =>
                table.HasCheckConstraint(
                    "CK_Users_UserRole",
                    "[UserRole] IN (N'Admin', N'Student')"));
            entity.HasKey(user => user.UserId);
            entity.HasIndex(user => user.Username).IsUnique();
            entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
            entity.Property(user => user.FullName).HasMaxLength(201).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(256);
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.UserRole).HasMaxLength(20).HasDefaultValue("Student");
            entity.Property(user => user.IsActive).HasDefaultValue(true);
            entity.Property(user => user.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<StudentProfile>(entity =>
        {
            entity.Property(profile => profile.BusinessName).HasMaxLength(100);
            entity.Property(profile => profile.Owner).HasMaxLength(2100);
            entity.ToTable("StudentProfiles");
            entity.HasKey(profile => profile.StudentProfileId);
            entity.HasIndex(profile => profile.UserId).IsUnique();
            entity.Property(profile => profile.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.MiddleName).HasMaxLength(100);
            entity.Property(profile => profile.LastName).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.BirthDate).HasColumnType("date");
            entity.Property(profile => profile.ProfilePhotoUrl).HasMaxLength(500);
            entity.Property(profile => profile.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasOne(profile => profile.User)
                .WithOne(user => user.StudentProfile)
                .HasForeignKey<StudentProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EducationHistory>(entity =>
        {
            entity.ToTable("EducationHistory", table =>
                table.HasCheckConstraint(
                    "CK_EducationHistory_Level",
                    "[EducationLevel] IN (N'Elementary', N'Junior High', N'Senior High', N'College')"));
            entity.HasKey(education => education.EducationHistoryId);
            entity.Property(education => education.Grade).HasMaxLength(50).IsRequired();
            entity.Property(education => education.StrandTrack).HasMaxLength(200);
            entity.Property(education => education.CollegeCourse).HasMaxLength(200);
            entity.HasIndex(education => new
            {
                education.StudentProfileId,
                education.EducationLevel,
                education.Grade
            }).IsUnique();
            entity.Property(education => education.EducationLevel).HasMaxLength(30).IsRequired();
            entity.Property(education => education.SchoolName).HasMaxLength(200).IsRequired();
            entity.Property(education => education.DisplayOrder).HasDefaultValue(0);
            entity.HasOne(education => education.StudentProfile)
                .WithMany(profile => profile.EducationHistory)
                .HasForeignKey(education => education.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.ToTable("Skills");
            entity.HasKey(skill => skill.SkillId);
            entity.Property(skill => skill.Name).IsRequired();
            entity.Property(skill => skill.DisplayOrder).HasDefaultValue(0);
            entity.HasOne(skill => skill.StudentProfile)
                .WithMany(profile => profile.Skills)
                .HasForeignKey(skill => skill.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.ToTable("Hobbies");
            entity.HasKey(hobby => hobby.HobbyId);
            entity.Property(hobby => hobby.Name).IsRequired();
            entity.Property(hobby => hobby.DisplayOrder).HasDefaultValue(0);
            entity.HasOne(hobby => hobby.StudentProfile)
                .WithMany(profile => profile.Hobbies)
                .HasForeignKey(hobby => hobby.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(project => project.ImageId).HasMaxLength(100);
            entity.ToTable("Projects");
            entity.HasKey(project => project.ProjectId);
            entity.Property(project => project.Title).HasMaxLength(200).IsRequired();
            entity.Property(project => project.ProjectUrl).HasMaxLength(500);
            entity.Property(project => project.DisplayOrder).HasDefaultValue(0);
            entity.HasOne(project => project.StudentProfile)
                .WithMany(profile => profile.Projects)
                .HasForeignKey(project => project.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SocialLink>(entity =>
        {
            entity.ToTable("SocialLinks");
            entity.HasKey(link => link.SocialLinkId);
            entity.Property(link => link.Label).HasMaxLength(100).IsRequired();
            entity.Property(link => link.Url).HasMaxLength(500).IsRequired();
            entity.Property(link => link.IsVisible).HasDefaultValue(true);
            entity.Property(link => link.DisplayOrder).HasDefaultValue(0);
            entity.HasOne(link => link.StudentProfile)
                .WithMany(profile => profile.SocialLinks)
                .HasForeignKey(link => link.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ContactInformation>(entity =>
        {
            entity.ToTable("ContactInformation");
            entity.HasKey(contact => contact.ContactInformationId);
            entity.HasIndex(contact => contact.StudentProfileId).IsUnique();
            entity.Property(contact => contact.Email).HasMaxLength(256);
            entity.Property(contact => contact.Phone).HasMaxLength(50);
            entity.Property(contact => contact.Address).HasMaxLength(500);
            entity.Property(contact => contact.Website).HasMaxLength(500);
            entity.Property(contact => contact.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasOne(contact => contact.StudentProfile)
                .WithOne(profile => profile.ContactInformation)
                .HasForeignKey<ContactInformation>(contact => contact.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
