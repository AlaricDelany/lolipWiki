using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public interface IUserRepository
    {
        IQueryable<UserEM> GetAll(ILolipWikiDbContext       dbContext);
        UserEM             Get(ILolipWikiDbContext          context,   long twitchUserId);
        UserEM             GetOrDefault(ILolipWikiDbContext dbContext, long twitchUserId);

        UserEM AddOrUpdateUser(
            ILolipWikiDbContext dbContext,
            long                userId,
            string              name,
            string              displayName,
            string              email,
            string              profilePictureImagePath,
            int                 subscriptionState
        );

        UserEM UpdateUserName(
            ILolipWikiDbContext dbContext,
            UserEM              targetUser,
            string              name,
            string              displayName,
            string              profilePictureImagePath
        );

        UserEM ToggleLock(ILolipWikiDbContext dbContext, long requestorUserId, long twitchUserId);
    }
}