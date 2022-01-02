using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Models;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IConfigurationLogic
    {
        AccessControlType GetReadControlType(ILolipWikiDbContext  dbContext, IRequestor requestor);
        AccessControlType GetWriteControlType(ILolipWikiDbContext dbContext, IRequestor requestor);

        void Update(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            AccessControlType   readControlType,
            AccessControlType   writeControlType
        );
    }
}