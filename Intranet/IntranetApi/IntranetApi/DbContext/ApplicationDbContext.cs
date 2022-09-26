using IntranetApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace IntranetApi.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>
    {
        public ApplicationDbContext
           (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override DbSet<User> Users { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        public override DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<RoleDepartment> RoleDepartments { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandEmployee> BrandEmployees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<StaffRecord> StaffRecords { get; set; }
        public DbSet<StaffRecordDocument> StaffRecordDocuments { get; set; }
        public DbSet<EmployeeImportHistory> EmployeeImportHistories { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaims");

            modelBuilder.Entity<User>().HasIndex(p => p.EmployeeCode).IsUnique();
            modelBuilder.Entity<StaffRecord>().Property(p=>p.Fine).HasPrecision(18, 2);
            modelBuilder.Entity<StaffRecord>().Property(p=>p.CalculationAmount).HasPrecision(18, 2);
            modelBuilder.Entity<BrandEmployee>().HasKey(p => new { p.BrandId, p.EmployeeId });
            modelBuilder.Entity<RoleDepartment>().HasKey(p => new { p.RoleId, p.DepartmentId });

            modelBuilder.Entity<StaffRecordDocument>()
                    .HasOne(p=>p.StaffRecord)
                    .WithMany(p=>p.StaffRecordDocuments)
                    .HasForeignKey(p=>p.StaffRecordId)
                    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffRecord>()
                    .HasOne(p => p.Employee)
                    .WithMany(p => p.StaffRecords)
                    .HasForeignKey(p => p.EmployeeId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BrandEmployee>()
                    .HasOne(bc => bc.Brand)
                    .WithMany(b => b.BrandEmployees)
                    .HasForeignKey(bc => bc.BrandId);

            modelBuilder.Entity<BrandEmployee>()
                    .HasOne(bc => bc.Employee)
                    .WithMany(b => b.BrandEmployees)
                    .HasForeignKey(bc => bc.EmployeeId);

            modelBuilder.Entity<UserRole>()
                    .HasOne(p => p.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(p => p.UserId);


            modelBuilder.Entity<UserRole>()
                    .HasOne(p => p.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(p => p.RoleId);


            modelBuilder.Entity<RoleDepartment>()
                    .HasOne(p => p.Role)
                    .WithMany(p => p.RoleDepartments)
                    .HasForeignKey(p => p.RoleId);

            modelBuilder.Entity<RoleDepartment>()
                    .HasOne(p => p.Department)
                    .WithMany(p => p.RoleDepartments)
                    .HasForeignKey(p => p.DepartmentId);

            modelBuilder.Entity<RoleClaim>()
                   .HasOne(p => p.Role)
                   .WithMany(b => b.RoleClaims)
                   .HasForeignKey(p => p.RoleId);
        }
    }
}
