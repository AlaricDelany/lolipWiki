using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.User
{
    [Authorize]
    public class UserDetailModel : BasePageModel
    {
        private readonly IUserManagementLogic _userManagementLogic;

        public UserDetailModel(ILolipWikiDbContext dbContext, IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic) : base(dbContext,
                                                                                                                                                       userManagementLogic,
                                                                                                                                                       accessControlLogic,
                                                                                                                                                       true
                                                                                                                                                      )
        {
            _userManagementLogic = userManagementLogic;
        }

        [BindProperty]
        public bool IsAdmin { get; set; }

        [BindProperty]
        public bool IsArticleManager { get; set; }

        [BindProperty]
        public bool IsArticleReviewer { get; set; }

        [BindProperty]
        public bool IsUserManager { get; set; }

        public IUser UserToUpdate { get; set; }

        public IActionResult OnGet(long userId)
        {
            var user = _userManagementLogic.GetUser(DbContext, Requestor, userId);

            IsAdmin           = user.IsAdmin;
            IsUserManager     = user.IsUserManager;
            IsArticleManager  = user.IsArticleManager;
            IsArticleReviewer = user.IsArticleReviewer;
            UserToUpdate      = user;

            return Page();
        }

        public IActionResult OnPost(long userId)
        {
            if (!ModelState.IsValid)
                return RedirectToPage("Detail",
                                      new
                                      {
                                          userId
                                      }
                                     );

            _userManagementLogic.UpdateRoles(DbContext,
                                             Requestor,
                                             userId,
                                             IsAdmin,
                                             IsUserManager,
                                             IsArticleManager,
                                             IsArticleReviewer
                                            );

            return RedirectToPage("List");
        }
    }
}