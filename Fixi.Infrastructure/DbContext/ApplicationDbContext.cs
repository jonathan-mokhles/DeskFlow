using Fixi.Core.Domain.Entity;
using Fixi.Core.Domain.IdentityEntity;
using Fixi.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Fixi.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        // DbSets
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }
        public DbSet<TicketStatusHistory> TicketStatusHistory { get; set; }
        public DbSet<SLASetting> SLASettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ticket relationships
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ReportedBy)
                .WithMany()
                .HasForeignKey(t => t.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.LastModifiedBy)
                .WithMany()
                .HasForeignKey(t => t.LastModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketAttachment relationships
            modelBuilder.Entity<TicketAttachment>()
                .HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketComment relationships
            modelBuilder.Entity<TicketComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // TicketStatusHistory relationships
            modelBuilder.Entity<TicketStatusHistory>()
                .HasOne(s => s.ChangedBy)
                .WithMany()
                .HasForeignKey(s => s.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);

            CategorySeed(modelBuilder);
            SLASettingSeed(modelBuilder);
            DepartmentSeed(modelBuilder);
            RolesSeed(modelBuilder);
        }

        


        private void DepartmentSeed(ModelBuilder builder)
        {
            builder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    Name = "IT Support"
                },
                new Department
                {
                    Id = 2,
                    Name = "Human Resources"
                },
                new Department
                {
                    Id = 3,
                    Name = "Finance"
                },
                new Department
                {
                    Id = 4,
                    Name = "Facilities"
                }
            );
        }
        private void SLASettingSeed(ModelBuilder builder)
        {
            builder.Entity<SLASetting>().HasData(
                // 
                new SLASetting
                {
                    Id = 1,
                    Priority = TicketPriority.Low,
                    ResponseTimeMinutes = 1440, // 24 hours
                    ResolutionTimeMinutes = 2400 // 5 working days
                },

                //  Medium Priority
                new SLASetting
                {
                    Id = 2,
                    Priority = TicketPriority.Medium,
                    ResponseTimeMinutes = 480, // 8 hours
                    ResolutionTimeMinutes = 960 // 2 working days
                },

                // High Priority
                new SLASetting
                {
                    Id = 3,
                    Priority = TicketPriority.High,
                    ResponseTimeMinutes = 120, // 2 hours
                    ResolutionTimeMinutes = 480 // 8 hours
                },

                // Critical Priority
                new SLASetting
                {
                    Id = 4,
                    Priority = TicketPriority.Critical,
                    ResponseTimeMinutes = 30, // 30 minutes
                    ResolutionTimeMinutes = 240 // 4 hours
                }
            );
        }
        private void CategorySeed(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                // IT Support Categories
                new Category
                {
                    Id = 1,
                    Name = "Hardware",
                    Description = "Computer, laptop, printer, and other hardware issues",
                    DepartmentId = 1
                },
                new Category
                {
                    Id = 2,
                    Name = "Software",
                    Description = "Application installation, licensing, and software issues",
                    DepartmentId = 1
                },
                new Category
                {
                    Id = 3,
                    Name = "Network",
                    Description = "Internet connectivity, VPN, and network access issues",
                    DepartmentId = 1
                },
                new Category
                {
                    Id = 4,
                    Name = "Email & Communication",
                    Description = "Email, Teams, Slack, and communication tool issues",
                    DepartmentId = 1
                },
                new Category
                {
                    Id = 5,
                    Name = "Account & Access",
                    Description = "Password resets, account creation, and permission requests",
                    DepartmentId = 1
                },

                // HR Categories
                new Category
                {
                    Id = 6,
                    Name = "Payroll",
                    Description = "Salary, benefits, and payroll related issues",
                    DepartmentId = 2
                },
                new Category
                {
                    Id = 7,
                    Name = "Leave & Attendance",
                    Description = "Leave requests, attendance tracking, and time-off issues",
                    DepartmentId = 2
                },

                // Finance Categories
                new Category
                {
                    Id = 10,
                    Name = "Payment Processing",
                    Description = "Incidents related to failed, delayed, or incorrect financial transactions.",
                    DepartmentId = 3
                },
                new Category
                {
                    Id = 11,
                    Name = "Invoice & Billing",
                    Description = "Problems generating, viewing, or processing invoices and billing records.",
                    DepartmentId = 3
                },


                // Facilities Categories
                new Category
                {
                    Id = 13,
                    Name = "Office Maintenance",
                    Description = "Building maintenance, repairs, and cleaning",
                    DepartmentId = 4
                },
                new Category
                {
                    Id = 15,
                    Name = "Security & Access",
                    Description = "Building access, key cards, and security issues",
                    DepartmentId = 4
                }

            );
        }
        private void RolesSeed(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "a1b2c3d4-e5f6-7890-abcd-1234567890ab"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    ConcurrencyStamp = "b2c3d4e5-f6a7-8901-bcde-2345678901bc"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Technician",
                    NormalizedName = "TECHNICIAN",
                    ConcurrencyStamp = "c3d4e5f6-a7b8-9012-cdef-3456789012cd"
                },
                new IdentityRole
                {
                    Id = "4",
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "d4e5f6a7-b8c9-0123-def0-4567890123de"
                }
);

        }
    }
}
