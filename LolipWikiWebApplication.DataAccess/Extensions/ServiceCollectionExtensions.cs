using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LolipWikiWebApplication.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IConfigurationRepository, ConfigurationRepository>();

            return services;
        }
    }
}