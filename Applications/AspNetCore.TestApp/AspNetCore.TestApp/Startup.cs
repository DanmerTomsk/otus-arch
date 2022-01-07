using AspNetCore.TestApp.Db;
using AspNetCore.TestApp.Middlewares.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Prometheus;
using System;
using System.Data.Common;

namespace AspNetCore.TestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(GetDbConnection()));

            services.AddHealthChecks();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHttpMetrics();
            app.UsePrometheusCustomMiddleware();

            var randomSeed = new Random(DateTime.Now.Millisecond).Next();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/server_ip", async context =>
                {
                    var httpConnectionFeature = context.Features.Get<IHttpConnectionFeature>();
                    var localIpAddress = httpConnectionFeature?.LocalIpAddress;

                    if (localIpAddress is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                    await context.Response.WriteAsync(localIpAddress.ToString());
                });

                endpoints.MapGet("/seed", async context =>
                {
                    await context.Response.WriteAsync(randomSeed.ToString());
                });

                endpoints.MapGet("/version", async context =>
                {
                    await context.Response.WriteAsync(1.ToString());
                });

                endpoints.MapGet("/health", async context =>
                {
                    await context.Response.WriteAsync("{\"status\": \"OK\"}");
                });

                endpoints.MapHealthChecks("/healthCheck");
            });
        }

        private DbConnection GetDbConnection()
        {
            var configConn = Configuration.GetConnectionString("DbConnection");
            if (string.IsNullOrWhiteSpace(configConn))
            {
                throw new InvalidOperationException("Can't get connection string from config [DbConnection]");
            }

            var connectionBuilder = new NpgsqlConnectionStringBuilder(configConn);

#if !DEBUG
            if (!string.IsNullOrWhiteSpace(connectionBuilder.Password))
            {
                throw new InvalidOperationException("Remove password from connection string!");
            }

            var user = Environment.GetEnvironmentVariable("DB_USER");
            if (string.IsNullOrWhiteSpace(user)) 
                throw new InvalidOperationException("Can't get DB_USER from env");

            var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrWhiteSpace(pass)) 
                throw new InvalidOperationException("Can't get DB_PASSWORD from env");

            connectionBuilder.Username = user;
            connectionBuilder.Password = pass;
#endif

            return new NpgsqlConnection(connectionBuilder.ConnectionString);
        }
    }
}
