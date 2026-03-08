using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fixi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changed_TicketAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromStatus",
                table: "TicketStatusHistory");

            migrationBuilder.DropColumn(
                name: "ToStatus",
                table: "TicketStatusHistory");

            migrationBuilder.AddColumn<string>(
                name: "ChangeType",
                table: "TicketStatusHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "TicketStatusHistory",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "TicketStatusHistory",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeType",
                table: "TicketStatusHistory");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "TicketStatusHistory");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "TicketStatusHistory");

            migrationBuilder.AddColumn<int>(
                name: "FromStatus",
                table: "TicketStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToStatus",
                table: "TicketStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
