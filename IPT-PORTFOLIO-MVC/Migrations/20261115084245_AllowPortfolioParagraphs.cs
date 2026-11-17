// Migration ito para payagan ang mas mahabang about, skills, at hobbies text.
﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPT_PORTFOLIO_MVC.Migrations
{
    public partial class AllowPortfolioParagraphs : Migration
    {
        // Ito ang pagbabago sa database kapag in-apply ang migration.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Palitan ang column definition ng AboutBusiness.
            migrationBuilder.AlterColumn<string>(
                name: "AboutBusiness",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            // Palitan ang column definition ng Name.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            // Palitan ang column definition ng Name.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hobbies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
        // Ito ang pagbabalik ng schema kapag ni-rollback ang migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Palitan ang column definition ng AboutBusiness.
            migrationBuilder.AlterColumn<string>(
                name: "AboutBusiness",
                table: "StudentProfiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Palitan ang column definition ng Name.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Skills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Palitan ang column definition ng Name.
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hobbies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
