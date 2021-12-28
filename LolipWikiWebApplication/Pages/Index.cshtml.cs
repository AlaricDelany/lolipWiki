using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Authorization;

namespace LolipWikiWebApplication.Pages
{
    [Authorize]
    public class IndexModel : BasePageModel
    {
        public IndexModel(IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic) : base(userManagementLogic, accessControlLogic, false)
        {
        }

        public void OnGet()
        {
        }
    }
}