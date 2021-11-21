using AspNetCore.TestApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.TestApp.Db
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) :base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCollation("my_collation", locale: "en-u-ks-primary", provider: "icu", deterministic: false);
            modelBuilder.UseDefaultColumnCollation("my_collation");
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //=> optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=otusDb;Username=postgres;Password=postgres");
    }
}
