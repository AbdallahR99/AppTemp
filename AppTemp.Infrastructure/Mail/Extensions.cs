using AppTemp.Core.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace AppTemp.Infrastructure.Mail;
internal static class Extensions
{
    internal static IServiceCollection ConfigureMailing(this IServiceCollection services)
    {
        services.AddTransient<IMailService, SmtpMailService>();
        services.AddOptions<MailOptions>().BindConfiguration(nameof(MailOptions));
        return services;
    }
}
