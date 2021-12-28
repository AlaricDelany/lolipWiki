using System;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LolipWikiWebApplication.PageModels
{
    public class BasePageModel : PageModel
    {
        private readonly IUserManagementLogic _userManagementLogic;
        private readonly IAccessControlLogic  _accessControlLogic;
        private readonly bool                 _isEditView;

        public BasePageModel(IUserManagementLogic userManagementLogic, IAccessControlLogic accessControlLogic, bool isEditView)
        {
            _userManagementLogic = userManagementLogic;
            _accessControlLogic  = accessControlLogic;
            _isEditView          = isEditView;
        }

        public    IRequestor Requestor   { get; private set; }
        public    IUser      TwitchUser  { get; private set; }
        protected string     AccessToken { get; private set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var twitchUser = User.ToTwitchUser();

            AccessToken = await GetAccessTokenAsync();
            Requestor   = twitchUser;
            TwitchUser  = _userManagementLogic.GetUser(twitchUser, twitchUser.Id);

            if (_isEditView)
                _accessControlLogic.EnsureWriteIsAllowed(TwitchUser);
            else
                _accessControlLogic.EnsureReadIsAllowed(TwitchUser);


            await base.OnPageHandlerExecutionAsync(context, next);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            return token;
        }
    }
}