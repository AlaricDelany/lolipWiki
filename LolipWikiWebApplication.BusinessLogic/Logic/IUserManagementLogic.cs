using System.Collections.Generic;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IUserManagementLogic
    {
        IUser UpdateRoles(
            IRequestor requestor,
            long       userId,
            bool       isAdmin,
            bool       isUserManager,
            bool       isArticleManager,
            bool       isArticleReviewer
        );

        /// <summary>
        ///     Has to be TwitchUser and not IUser cause stupid Json Serializer will ignore all Properties from IRequestor
        /// </summary>
        /// <param name="requestor"></param>
        /// <returns></returns>
        IEnumerable<UserBM> GetAll(IRequestor requestor);

        IUser       GetUser(IRequestor              requestor, long   id);
        Task<IUser> AddOrUpdateUserAsync(IRequestor requestor, string accessToken);
        Task<IUser> UpdateUserNameAsync(IRequestor  requestor, string accessToken, long userId);
        IUser       ToggleLock(IRequestor           requestor, long   userId);
        Task<IUser> ImportAsync(IRequestor          requestor, string accessToken, string userName);
    }
}