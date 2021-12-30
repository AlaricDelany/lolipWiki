using Microsoft.Extensions.DependencyInjection;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwitchClient(this IServiceCollection services)
        {
            services.AddHttpClient<ITwitchClient, TwitchClient>();
            services.AddSingleton<ITwitchClient, TwitchClient>();

            return services;
        }
    }
}