using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Models;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public interface IConfigurationRepository
    {
        ConfigurationEM Get();
        ConfigurationEM Get(ILolipWikiDbContext dbContext);
    }
}