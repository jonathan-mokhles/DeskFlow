using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fixi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Roleid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_Roleid",
                        column: x => x.Roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departmentid = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Departments_Departmentid",
                        column: x => x.Departmentid,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Departmentid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_Categories_Departments_Departmentid",
                        column: x => x.Departmentid,
                        principalTable: "Departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Roleid = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.Userid, x.Roleid });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_Roleid",
                        column: x => x.Roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.Userid, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SLASettings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ResponseTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    Categoryid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLASettings", x => x.id);
                    table.ForeignKey(
                        name: "FK_SLASettings_Categories_Categoryid",
                        column: x => x.Categoryid,
                        principalTable: "Categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Categoryid = table.Column<int>(type: "int", nullable: false),
                    ReportedByid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedToid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SLADeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SLABreached = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedByid = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_AssignedToid",
                        column: x => x.AssignedToid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_LastModifiedByid",
                        column: x => x.LastModifiedByid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_AspNetUsers_ReportedByid",
                        column: x => x.ReportedByid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Categories_Categoryid",
                        column: x => x.Categoryid,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketAttachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Ticketid = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadedByid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAttachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_TicketAttachments_AspNetUsers_UploadedByid",
                        column: x => x.UploadedByid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketAttachments_Tickets_Ticketid",
                        column: x => x.Ticketid,
                        principalTable: "Tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketComments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Ticketid = table.Column<int>(type: "int", nullable: false),
                    Userid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketComments", x => x.id);
                    table.ForeignKey(
                        name: "FK_TicketComments_AspNetUsers_Userid",
                        column: x => x.Userid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketComments_Tickets_Ticketid",
                        column: x => x.Ticketid,
                        principalTable: "Tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatusHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:identity", "1, 1"),
                    Ticketid = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedByid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatusHistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_AspNetUsers_ChangedByid",
                        column: x => x.ChangedByid,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketStatusHistory_Tickets_Ticketid",
                        column: x => x.Ticketid,
                        principalTable: "Tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "a1b2c3d4-e5f6-7890-abcd-1234567890ab", "Admin", "ADMIN" },
                    { "2", "b2c3d4e5-f6a7-8901-bcde-2345678901bc", "Manager", "MANAGER" },
                    { "3", "c3d4e5f6-a7b8-9012-cdef-3456789012cd", "Technician", "TECHNICIAN" },
                    { "4", "d4e5f6a7-b8c9-0123-def0-4567890123de", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "id", "Name" },
                values: new object[,]
                {
                    { 1, "IT Support" },
                    { 2, "Human Resources" },
                    { 3, "Finance" },
                    { 4, "Facilities" }
                });

            migrationBuilder.InsertData(
                table: "SLASettings",
                columns: new[] { "id", "Categoryid", "Priority", "ResolutionTimeMinutes", "ResponseTimeMinutes" },
                values: new object[,]
                {
                    { 1, null, 1, 2400, 1440 },
                    { 2, null, 2, 960, 480 },
                    { 3, null, 3, 480, 120 },
                    { 4, null, 4, 240, 30 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "id", "Departmentid", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Computer, laptop, printer, and other hardware issues", "Hardware" },
                    { 2, 1, "Application installation, licensing, and software issues", "Software" },
                    { 3, 1, "Internet connectivity, VPN, and network access issues", "Network" },
                    { 4, 1, "Email, Teams, Slack, and communication tool issues", "Email & Communication" },
                    { 5, 1, "Password resets, account creation, and permission requests", "Account & Access" },
                    { 6, 2, "Salary, benefits, and payroll related issues", "Payroll" },
                    { 7, 2, "Leave requests, attendance tracking, and time-off issues", "Leave & Attendance" },
                    { 10, 3, "Incidents related to failed, delayed, or incorrect financial transactions.", "Payment Processing" },
                    { 11, 3, "Problems generating, viewing, or processing invoices and billing records.", "Invoice & Billing" },
                    { 13, 4, "Building maintenance, repairs, and cleaning", "Office Maintenance" },
                    { 15, 4, "Building access, key cards, and security issues", "Security & Access" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_Roleid",
                table: "AspNetRoleClaims",
                column: "Roleid");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_Userid",
                table: "AspNetUserClaims",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_Userid",
                table: "AspNetUserLogins",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_Roleid",
                table: "AspNetUserRoles",
                column: "Roleid");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Departmentid",
                table: "AspNetUsers",
                column: "Departmentid");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Departmentid",
                table: "Categories",
                column: "Departmentid");

            migrationBuilder.CreateIndex(
                name: "IX_SLASettings_Categoryid",
                table: "SLASettings",
                column: "Categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_Ticketid",
                table: "TicketAttachments",
                column: "Ticketid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_UploadedByid",
                table: "TicketAttachments",
                column: "UploadedByid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_Ticketid",
                table: "TicketComments",
                column: "Ticketid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_Userid",
                table: "TicketComments",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToid",
                table: "Tickets",
                column: "AssignedToid");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Categoryid",
                table: "Tickets",
                column: "Categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_LastModifiedByid",
                table: "Tickets",
                column: "LastModifiedByid");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ReportedByid",
                table: "Tickets",
                column: "ReportedByid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_ChangedByid",
                table: "TicketStatusHistory",
                column: "ChangedByid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistory_Ticketid",
                table: "TicketStatusHistory",
                column: "Ticketid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "SLASettings");

            migrationBuilder.DropTable(
                name: "TicketAttachments");

            migrationBuilder.DropTable(
                name: "TicketComments");

            migrationBuilder.DropTable(
                name: "TicketStatusHistory");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
