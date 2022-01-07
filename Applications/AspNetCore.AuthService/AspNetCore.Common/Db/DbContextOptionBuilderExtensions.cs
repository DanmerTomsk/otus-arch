using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AspNetCore.Common.Db
{
    public static class DbContextOptionBuilderExtensions
    {
        public static void ConfigureConnection(this DbContextOptionsBuilder builder, DbConnectionInfo connectionInfo)
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder();
            connectionBuilder.Host = connectionInfo.DbHost;
            connectionBuilder.Port = connectionInfo.DbPort;
            connectionBuilder.Database = connectionInfo.DbName;
            connectionBuilder.Username = connectionInfo.DbUsername;
            connectionBuilder.Password = connectionInfo.DbPassword;

            builder.UseNpgsql(new NpgsqlConnection(connectionBuilder.ConnectionString));
        }
    }
}
