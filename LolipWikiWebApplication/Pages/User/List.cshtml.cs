using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.User
{
    [Authorize]
    public class UserListModel : BasePageModel
    {
        private readonly IUserManagementLogic _userManagementLogic;

        public UserListModel(ILolipWikiDbContext dbContext, IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic) : base(dbContext,
                                                                                                                                                     userManagementLogic,
                                                                                                                                                     accessControlLogic,
                                                                                                                                                     false
                                                                                                                                                    )
        {
            _userManagementLogic = userManagementLogic;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetUpdateName(long userId)
        {
            var user = await _userManagementLogic.UpdateUserNameAsync(DbContext,
                                                                      Requestor,
                                                                      AccessToken,
                                                                      userId
                                                                     );

            return RedirectToPage("List");
        }

        public IActionResult OnGetToggleLock(long userId)
        {
            _userManagementLogic.ToggleLock(DbContext, Requestor, userId);

            return RedirectToPage("List");
        }

        public async Task<IActionResult> OnPostImportAsync(UserImportModel importModel)
        {
            var user = await _userManagementLogic.ImportAsync(DbContext,
                                                              Requestor,
                                                              AccessToken,
                                                              importModel.UserName
                                                             );

            return RedirectToPage("Detail",
                                  new
                                  {
                                      userId = user.Id
                                  }
                                 );
        }
    }
}