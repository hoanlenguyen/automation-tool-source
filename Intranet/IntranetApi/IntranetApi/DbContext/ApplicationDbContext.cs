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

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<RoleClaim> RoleClaim { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Brand> Brand { get; set; }
        public DbSet<BrandEmployee> BrandEmployee { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Rank> Rank { get; set; }
        public DbSet<EmployeeImportHistory> EmployeeImportHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<RoleClaim>().ToTable("RoleClaim");

            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Employee>().HasIndex(p=>p.EmployeeCode).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(p=>p.IdNumber).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(p=>p.BackendUser).IsUnique();

            modelBuilder.Entity<Bank>().ToTable("Bank");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Rank>().ToTable("Rank");

            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<BrandEmployee>().ToTable("BrandEmployee");

            modelBuilder.Entity<EmployeeImportHistory>().ToTable("EmployeeImportHistory");
        }
    }
}
