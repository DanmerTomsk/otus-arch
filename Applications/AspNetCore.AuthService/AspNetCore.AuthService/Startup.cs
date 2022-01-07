using AspNetCore.AuthService.Logic;
using AspNetCore.AuthService.Settings;
using AspNetCore.Common.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AspNetCore.AuthService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AccountsDbContext>(options => options.ConfigureConnection(GetDbConnection()));
            services.AddHealthChecks();

            services.AddOptions<TokenOptions>()
                .Bind(Configuration.GetSection(TokenOptions.SettingsSectionName), options => options.BindNonPublicProperties = true)
                .ValidateDataAnnotations();
            services.AddSingleton<AuthJwtManager>();
            services.AddSingleton<SessionStorage>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        private DbConnectionInfo GetDbConnection()
        {
            var configConnSection = Configuration.GetSection("ConnectionInfo");

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
