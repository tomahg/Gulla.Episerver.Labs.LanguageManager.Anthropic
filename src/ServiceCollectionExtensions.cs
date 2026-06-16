using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gulla.Episerver.Labs.LanguageManager.Anthropic
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLanguageManagerAnthropic(this IServiceCollection services)
        {
            return AddLanguageManagerAnthropic(services, _ => { });
        }

        public static IServiceCollection AddLanguageManagerAnthropic(this IServiceCollection services, Action<LanguageManagerAnthropicOptions> setupAction)
        {
            services.AddTransient<LanguageManagerAnthropicService, LanguageManagerAnthropicService>();

            services.AddOptions<LanguageManagerAnthropicOptions>().Configure<IConfiguration>((options, configuration) =>
            {
                setupAction(options);
                configuration.GetSection("Gulla:LanguageManagerAnthropic").Bind(options);
            });

            return services;
        }
    }
}
