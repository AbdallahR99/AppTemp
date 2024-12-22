using AppTemp.Core.Exceptions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AppTemp.Core.Persistence;
using AppTemp.Core.Jobs;
using AppTemp.Infrastructure.Persistence;
using AppTemp.Infrastructure.Jobs;

namespace AppTemp.Infrastructure.Jobs;

internal static class Extensions
{
    internal static IServiceCollection ConfigureJobs(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>() ??
            throw new AppException("database options cannot be null");

        services.AddHangfireServer(o =>
        {
            o.HeartbeatInterval = TimeSpan.FromSeconds(30);
            o.Queues = new string[] { "default", "email" };
            o.WorkerCount = 5;
            o.SchedulePollingInterval = TimeSpan.FromSeconds(30);
        });

        services.AddHangfire((provider, config) =>
        {
            switch (dbOptions.Provider.ToUpperInvariant())
            {
                //case DbProviders.PostgreSQL:
                //    config.UseSqlServerStorage(o =>
                //    {
                //        o.UseNpgsqlConnection(dbOptions.ConnectionString);
                //    });
                //    break;

                case DbProviders.MSSQL:
                    config.UseSqlServerStorage(dbOptions.ConnectionString);
                    break;

                default:
                    throw new AppException($"hangfire storage provider {dbOptions.Provider} is not supported");
            }

            config.UseFilter(new AppJobFilter(provider));
            config.UseFilter(new LogJobFilter());
        });

        services.AddTransient<IJobService, HangfireService>();
        return services;
    }

    internal static IApplicationBuilder UseJobDashboard(this IApplicationBuilder app, IConfiguration config)
    {
        var hangfireOptions = config.GetSection(nameof(HangfireOptions)).Get<HangfireOptions>() ?? new HangfireOptions();
        var dashboardOptions = new DashboardOptions();
        dashboardOptions.AppPath = "https://dubaidashboard.qa.dubai.qov.ae/";
        dashboardOptions.Authorization = new[]
        {
           new HangfireCustomBasicAuthenticationFilter
           {
                User = hangfireOptions.UserName!,
                Pass = hangfireOptions.Password!
           }
        };

        return app.UseHangfireDashboard(hangfireOptions.Route, dashboardOptions);
    }
}
