namespace AspNetCore.Common.Db
{
    public sealed class DbConnectionInfo
    {
        public DbConnectionInfo(string dbHost, int dbPort, string dbName, string dbUsername, string dbPassword)
        {
            DbHost = dbHost;
            DbPort = dbPort;
            DbName = dbName;
            DbUsername = dbUsername;
            DbPassword = dbPassword;
        }

        internal string DbHost { get; }

        internal int DbPort { get; }

        internal string DbName { get; }

        internal string DbUsername { get; }

        internal string DbPassword { get; }
    }
}
