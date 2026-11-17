// Migration ito para payagan ang mas mahabang products at services text.
﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPT_PORTFOLIO_MVC.Migrations
{
    public partial class ExpandOfferingDescriptions : Migration
    {
        // Ito ang pagbabago sa database kapag in-apply ang migration.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Palitan ang column definition ng Offering.
            migrationBuilder.AlterColumn<string>(
                name: "Offering",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2100)",
                oldMaxLength: 2100,
                oldNullable: true);
        }
        // Ito ang pagbabalik ng schema kapag ni-rollback ang migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Palitan ang column definition ng Offering.
            migrationBuilder.AlterColumn<string>(
                name: "Offering",
                table: "StudentProfiles",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
