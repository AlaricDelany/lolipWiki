using System.Diagnostics.CodeAnalysis;
using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.Exceptions
{
    [SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class UserRoleMissingException : BusinessLayerException
    {
        public UserRoleMissingException(long userId) : base(BusinessLayerErrorCode.UserRoleMissing, GetMessage(userId))
        {
        }

        public static string GetMessage(long userId)
        {
            return $"The User with the TwitchUserId:{userId} has no Roles.";
        }
    }
}