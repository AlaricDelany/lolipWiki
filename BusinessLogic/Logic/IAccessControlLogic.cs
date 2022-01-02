using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IAccessControlLogic
    {
        void EnsureIsAllowed(ILolipWikiDbContext      dbContext, IRequestor requestor, string roleShortName);
        void EnsureReadIsAllowed(ILolipWikiDbContext  dbContext, IUser      user);
        void EnsureWriteIsAllowed(ILolipWikiDbContext dbContext, IUser      user);
    }
}