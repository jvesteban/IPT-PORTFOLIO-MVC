// Ito ang unang migration para buuin ang accounts at portfolio tables.
﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPT_PORTFOLIO_MVC.Migrations
{
    public partial class CreatePortfolioTables : Migration
    {
        // Ito ang pagbabago sa database kapag in-apply ang migration.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Buuin ang table na Users.
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(201)", maxLength: 201, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Student"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.CheckConstraint("CK_Users_UserRole", "[UserRole] IN (N'Admin', N'Student')");
                });

            // Buuin ang table na StudentProfiles.
            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    StudentProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CareerGoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AboutBusiness = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Offering = table.Column<string>(type: "nvarchar(2100)", maxLength: 2100, nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(2100)", maxLength: 2100, nullable: true),
                    ProfilePhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.StudentProfileId);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na ContactInformation.
            migrationBuilder.CreateTable(
                name: "ContactInformation",
                columns: table => new
                {
                    ContactInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInformation", x => x.ContactInformationId);
                    table.ForeignKey(
                        name: "FK_ContactInformation_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na EducationHistory.
            migrationBuilder.CreateTable(
                name: "EducationHistory",
                columns: table => new
                {
                    EducationHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    EducationLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StrandTrack = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CollegeCourse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SchoolName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartYear = table.Column<short>(type: "smallint", nullable: true),
                    EndYear = table.Column<short>(type: "smallint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationHistory", x => x.EducationHistoryId);
                    table.CheckConstraint("CK_EducationHistory_Level", "[EducationLevel] IN (N'Elementary', N'Junior High', N'Senior High', N'College')");
                    table.ForeignKey(
                        name: "FK_EducationHistory_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na Hobbies.
            migrationBuilder.CreateTable(
                name: "Hobbies",
                columns: table => new
                {
                    HobbyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hobbies", x => x.HobbyId);
                    table.ForeignKey(
                        name: "FK_Hobbies_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na Projects.
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na Skills.
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                    table.ForeignKey(
                        name: "FK_Skills_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Buuin ang table na SocialLinks.
            migrationBuilder.CreateTable(
                name: "SocialLinks",
                columns: table => new
                {
                    SocialLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentProfileId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialLinks", x => x.SocialLinkId);
                    table.ForeignKey(
                        name: "FK_SocialLinks_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalTable: "StudentProfiles",
                        principalColumn: "StudentProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            // I-set ang database index na IX_ContactInformation_StudentProfileId.
            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_StudentProfileId",
                table: "ContactInformation",
                column: "StudentProfileId",
                unique: true);

            // I-set ang database index na IX_EducationHistory_StudentProfileId_EducationLevel_Grade.
            migrationBuilder.CreateIndex(
                name: "IX_EducationHistory_StudentProfileId_EducationLevel_Grade",
                table: "EducationHistory",
                columns: new[] { "StudentProfileId", "EducationLevel", "Grade" },
                unique: true);

            // I-set ang database index na IX_Hobbies_StudentProfileId.
            migrationBuilder.CreateIndex(
                name: "IX_Hobbies_StudentProfileId",
                table: "Hobbies",
                column: "StudentProfileId");

            // I-set ang database index na IX_Projects_StudentProfileId.
            migrationBuilder.CreateIndex(
                name: "IX_Projects_StudentProfileId",
                table: "Projects",
                column: "StudentProfileId");

            // I-set ang database index na IX_Skills_StudentProfileId.
            migrationBuilder.CreateIndex(
                name: "IX_Skills_StudentProfileId",
                table: "Skills",
                column: "StudentProfileId");

            // I-set ang database index na IX_SocialLinks_StudentProfileId.
            migrationBuilder.CreateIndex(
                name: "IX_SocialLinks_StudentProfileId",
                table: "SocialLinks",
                column: "StudentProfileId");

            // I-set ang database index na IX_StudentProfiles_UserId.
            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId",
                unique: true);

            // I-set ang database index na IX_Users_Username.
            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }
        // Ito ang pagbabalik ng schema kapag ni-rollback ang migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Alisin ang table na ContactInformation.
            migrationBuilder.DropTable(
                name: "ContactInformation");

            // Alisin ang table na EducationHistory.
            migrationBuilder.DropTable(
                name: "EducationHistory");

            // Alisin ang table na Hobbies.
            migrationBuilder.DropTable(
                name: "Hobbies");

            // Alisin ang table na Projects.
            migrationBuilder.DropTable(
                name: "Projects");

            // Alisin ang table na Skills.
            migrationBuilder.DropTable(
                name: "Skills");

            // Alisin ang table na SocialLinks.
            migrationBuilder.DropTable(
                name: "SocialLinks");

            // Alisin ang table na StudentProfiles.
            migrationBuilder.DropTable(
                name: "StudentProfiles");

            // Alisin ang table na Users.
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
