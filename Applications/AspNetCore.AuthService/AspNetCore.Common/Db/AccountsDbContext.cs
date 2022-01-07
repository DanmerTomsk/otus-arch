using AspNetCore.Common;
using AspNetCore.Common.Helpers;
using AspNetCore.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Common.Db
{
    public class AccountsDbContext : DbContext
    {
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<DbUser> Users { get; set; } = null!;

        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.HasCollation("my_collation", locale: "en-u-ks-primary", provider: "icu", deterministic: false);
            modelBuilder.UseDefaultColumnCollation("my_collation");

            var adminRole = new Role { Id = 2, Name = Constants.AdminRoleName };
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User" },
                adminRole);

            modelBuilder.Entity<DbUser>().HasData(
                new DbUser { Id = 1, Username = "Admin", PasswordHash = PasswordHasher.GetHash("Admin123"), Email = "admin@mail.com", RoleId = adminRole.Id });

            base.OnModelCreating(modelBuilder);
        }
    }
}
