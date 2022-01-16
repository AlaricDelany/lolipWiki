using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;

namespace LolipWikiWebApplication.Pages
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(ILolipWikiDbContext dbContext, IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic) : base(dbContext,
                                                                                                                                                  userManagementLogic,
                                                                                                                                                  accessControlLogic,
                                                                                                                                                  false
                                                                                                                                                 )
        {
        }

        public void OnGet()
        {
        }
    }
}