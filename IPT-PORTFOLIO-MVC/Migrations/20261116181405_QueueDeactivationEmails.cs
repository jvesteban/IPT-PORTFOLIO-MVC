// Migration ito para itabi sa database ang pending deactivation emails.
﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPT_PORTFOLIO_MVC.Migrations
{
    public partial class QueueDeactivationEmails : Migration
    {
        // Ito ang pagbabago sa database kapag in-apply ang migration.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Buuin ang table na PendingDeactivationEmails.
            migrationBuilder.CreateTable(
                name: "PendingDeactivationEmails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingDeactivationEmails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PendingDeactivationEmails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }
        // Ito ang pagbabalik ng schema kapag ni-rollback ang migration.
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Alisin ang table na PendingDeactivationEmails.
            migrationBuilder.DropTable(
                name: "PendingDeactivationEmails");
        }
    }
}
