using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly ILolipWikiDbContext _dbContext;

        public ConfigurationRepository(ILolipWikiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ConfigurationEM Get(ILolipWikiDbContext dbContext)
        {
            return dbContext.Configurations.Single();
        }

        public ConfigurationEM Get()
        {
            return Get(_dbContext);
        }
    }
}