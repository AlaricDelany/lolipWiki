using LolipWikiWebApplication.BusinessLogic.Logic;
using Microsoft.Extensions.DependencyInjection;

namespace LolipWikiWebApplication.BusinessLogic.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogics(this IServiceCollection services)
        {
            services.AddTransient<IUserManagementLogic, UserManagementLogic>();
            services.AddTransient<IAccessControlLogic, AccessControlLogic>();
            services.AddTransient<IArticleLogic, ArticleLogic>();
            services.AddTransient<IConfigurationLogic, ConfigurationLogic>();


            return services;
        }
    }
}