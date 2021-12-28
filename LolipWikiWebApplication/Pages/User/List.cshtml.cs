using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.User
{
    [Authorize]
    public class UserListModel : BasePageModel
    {
        private readonly IUserManagementLogic _userManagementLogic;

        public UserListModel(IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic) : base(userManagementLogic, accessControlLogic, false)
        {
            _userManagementLogic = userManagementLogic;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetUpdateName(long userId)
        {
            var user = await _userManagementLogic.UpdateUserNameAsync(Requestor, AccessToken, userId);

            return RedirectToPage("List");
        }

        public IActionResult OnGetToggleLock(long userId)
        {
            _userManagementLogic.ToggleLock(Requestor, userId);

            return RedirectToPage("List");
        }

        public async Task<IActionResult> OnPostImportAsync(UserImportModel importModel)
        {
            var user = await _userManagementLogic.ImportAsync(Requestor, AccessToken, importModel.UserName);

            return RedirectToPage("Detail",
                                  new
                                  {
                                      userId = user.Id
                                  }
                                 );
        }
    }
}