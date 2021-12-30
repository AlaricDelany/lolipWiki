using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.Exceptions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class RoleNotFoundException : BusinessLayerException
    {
        public RoleNotFoundException(string roleShortName, long userId) : base(BusinessLayerErrorCode.Unauthorized, $"The Role: {roleShortName} was not found for User:{userId}")
        {
        }
    }
}