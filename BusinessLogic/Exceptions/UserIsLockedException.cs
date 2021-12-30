using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.Exceptions
{
    public class UserIsLockedException : BusinessLayerException
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
        public UserIsLockedException(long userId) : base(BusinessLayerErrorCode.UserIsLocked, $"The User with the TwitchUserId:{userId} is locked and not allowed to perform this Action.")
        {
        }
    }
}