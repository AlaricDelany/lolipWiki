using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Authorization;

namespace LolipWikiWebApplication.Pages.Article
{
    [Authorize]
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