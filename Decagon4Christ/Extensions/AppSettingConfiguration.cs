using Decagon4Christ.Model;

namespace Decagon4Christ.Extensions
{
    public static class AppsettingConfiguration
    {
        public static void AddAppSettingsConfig(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
        {
            var mailSettings = new EmailSettings();
            

            if (env.IsProduction())
            {
                mailSettings.Host = Environment.GetEnvironmentVariable("MailHost")!;
                mailSettings.Port = int.Parse(Environment.GetEnvironmentVariable("MailPort")!);
                mailSettings.DisplayName = Environment.GetEnvironmentVariable("MailDisplayName")!;
                mailSettings.Username = Environment.GetEnvironmentVariable("MailUsername")!;
                mailSettings.Password = Environment.GetEnvironmentVariable("MailPassword")!;
            }
            else
            {
                config.GetSection("EmailSettings").Bind(mailSettings);
                
            }

            services.AddSingleton(mailSettings);
        }
    }
}
