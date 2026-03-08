using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fixi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_SLA_in_Ticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatusHistory_AspNetUsers_ChangedById",
                table: "TicketStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatusHistory_Tickets_TicketId",
                table: "TicketStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketStatusHistory",
                table: "TicketStatusHistory");

            migrationBuilder.RenameTable(
                name: "TicketStatusHistory",
                newName: "TicketAuditLog");

            migrationBuilder.RenameColumn(
                name: "SLADeadline",
                table: "Tickets",
                newName: "SLAResponseDeadline");

            migrationBuilder.RenameColumn(
                name: "SLABreached",
                table: "Tickets",
                newName: "SLAResponseBreached");

            migrationBuilder.RenameColumn(
                name: "LoginProvIder",
                table: "AspNetUserTokens",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "ProvIderDisplayName",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "ProvIderKey",
                table: "AspNetUserLogins",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "LoginProvIder",
                table: "AspNetUserLogins",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "IX_TicketStatusHistory_TicketId",
                table: "TicketAuditLog",
                newName: "IX_TicketAuditLog_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketStatusHistory_ChangedById",
                table: "TicketAuditLog",
                newName: "IX_TicketAuditLog_ChangedById");

            migrationBuilder.AddColumn<bool>(
                name: "SLAResolutionBreached",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SLAResolutionDeadline",
                table: "Tickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketAuditLog",
                table: "TicketAuditLog",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "Incidents related to failed, delayed, or incorrect financial transactions.");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAuditLog_AspNetUsers_ChangedById",
                table: "TicketAuditLog",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAuditLog_Tickets_TicketId",
                table: "TicketAuditLog",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAuditLog_AspNetUsers_ChangedById",
                table: "TicketAuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAuditLog_Tickets_TicketId",
                table: "TicketAuditLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketAuditLog",
                table: "TicketAuditLog");

            migrationBuilder.DropColumn(
                name: "SLAResolutionBreached",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SLAResolutionDeadline",
                table: "Tickets");

            migrationBuilder.RenameTable(
                name: "TicketAuditLog",
                newName: "TicketStatusHistory");

            migrationBuilder.RenameColumn(
                name: "SLAResponseDeadline",
                table: "Tickets",
                newName: "SLADeadline");

            migrationBuilder.RenameColumn(
                name: "SLAResponseBreached",
                table: "Tickets",
                newName: "SLABreached");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                newName: "LoginProvIder");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                newName: "ProvIderDisplayName");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                newName: "ProvIderKey");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                newName: "LoginProvIder");

            migrationBuilder.RenameIndex(
                name: "IX_TicketAuditLog_TicketId",
                table: "TicketStatusHistory",
                newName: "IX_TicketStatusHistory_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketAuditLog_ChangedById",
                table: "TicketStatusHistory",
                newName: "IX_TicketStatusHistory_ChangedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketStatusHistory",
                table: "TicketStatusHistory",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "IncIdents related to failed, delayed, or incorrect financial transactions.");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatusHistory_AspNetUsers_ChangedById",
                table: "TicketStatusHistory",
                column: "ChangedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatusHistory_Tickets_TicketId",
                table: "TicketStatusHistory",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
