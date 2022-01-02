using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        public ConfigurationEM Get(ILolipWikiDbContext dbContext)
        {
            return dbContext.Configurations.Single();
        }
    }
}