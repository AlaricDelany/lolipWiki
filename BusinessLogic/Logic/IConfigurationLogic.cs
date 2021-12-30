using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess.Models;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IConfigurationLogic
    {
        AccessControlType GetReadControlType(IRequestor  requestor);
        AccessControlType GetWriteControlType(IRequestor requestor);
        void              Update(IRequestor              requestor, AccessControlType readControlType, AccessControlType writeControlType);
    }
}