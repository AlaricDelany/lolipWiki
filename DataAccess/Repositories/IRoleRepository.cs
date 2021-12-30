using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Models;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public interface IRoleRepository
    {
        RoleEM Get(ILolipWikiDbContext lolipWikiDbContext, string roleShortName);

        void Update(
            ILolipWikiDbContext dbContext,
            UserEM                user,
            string              roleShortName,
            bool                shouldHaveRole
        );
    }
}