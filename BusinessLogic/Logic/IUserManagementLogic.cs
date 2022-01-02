using System.Collections.Generic;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IUserManagementLogic
    {
        IUser UpdateRoles(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            long                userId,
            bool                isAdmin,
            bool                isUserManager,
            bool                isArticleManager,
            bool                isArticleReviewer
        );

        /// <summary>
        ///     Has to be TwitchUser and not IUser cause stupid Json Serializer will ignore all Properties from IRequestor
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="requestor"></param>
        /// <returns></returns>
        IEnumerable<UserBM> GetAll(ILolipWikiDbContext dbContext, IRequestor requestor);

        IUser       GetUser(ILolipWikiDbContext              dbContext, IRequestor requestor, long   id);
        Task<IUser> AddOrUpdateUserAsync(ILolipWikiDbContext dbContext, IRequestor requestor, string accessToken);

        Task<IUser> UpdateUserNameAsync(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            string              accessToken,
            long                userId
        );

        IUser ToggleLock(ILolipWikiDbContext dbContext, IRequestor requestor, long userId);

        Task<IUser> ImportAsync(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            string              accessToken,
            string              userName
        );
    }
}