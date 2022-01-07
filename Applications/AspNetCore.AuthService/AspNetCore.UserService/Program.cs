using AspNetCore.Common.Db;
using AspNetCore.UserService.Helpers;
using AspNetCore.UserService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;

namespace AspNetCore.UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.ConfigureAppConfiguration(configBuilder =>
                configBuilder.AddJsonFile("config.json", false, false));

            // Add services to the container.
            var services = builder.Services;
            services.AddDbContext<AccountsDbContext>(options => options.ConfigureConnection(GetDbConnection(builder.Configuration)));
            services.AddHealthChecks();
            ConfigureAuthentification(services, builder.Configuration);

            services.AddControllers();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            app.Run();
        }

        private static void ConfigureAuthentification(IServiceCollection services, ConfigurationManager configuration)
        {
            var tokenOptions = configuration
                .GetRequiredSection(TokenOptions.SettingsSectionName)
                .Get<TokenOptions>(options => options.BindNonPublicProperties = true);

            if (tokenOptions is null)
            {
                throw new InvalidOperationException("Can't get TokenOptions from services");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = tokenOptions.Issuer,

                        ValidateAudience = false,

                        ValidateLifetime = true,
                        IssuerSigningKey = RsaKeyCreator.GetPublicSecurityKey(tokenOptions.PublicKey),
                        ValidateIssuerSigningKey = true,
                    };
                });
        }

        private static DbConnectionInfo GetDbConnection(ConfigurationManager configuration)
        {
            var configConnSection = configuration.GetSection("ConnectionInfo");

            if (configConnSection is null)
            {
                throw new InvalidOperationException("Can't get connection string from config [ConnectionInfo]");
            }

            var host = configConnSection.GetValue<string>("DbHost");
            var port = configConnSection.GetValue<int>("DbPort");
            var dbName = configConnSection.GetValue<string>("DbName");

            var user = Environment.GetEnvironmentVariable("DB_USER");
            if (string.IsNullOrWhiteSpace(user))
                throw new InvalidOperationException("Can't get DB_USER from env");

            var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(pass))
                throw new InvalidOperationException("Can't get DB_PASSWORD from env");

            return new DbConnectionInfo(host, port, dbName, user, pass);
        }
    }
}


