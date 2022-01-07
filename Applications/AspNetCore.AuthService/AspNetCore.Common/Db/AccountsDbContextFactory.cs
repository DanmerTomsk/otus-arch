using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AspNetCore.Common.Db
{
    internal class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
    {
        public AccountsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AccountsDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=otusAuthDb;User ID=postgres;Password=postgres;");

            return new AccountsDbContext(optionsBuilder.Options);
        }
    }
}
