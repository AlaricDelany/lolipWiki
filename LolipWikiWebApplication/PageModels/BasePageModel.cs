using System;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
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

        public BasePageModel(
            ILolipWikiDbContext  dbContext,
            IUserManagementLogic userManagementLogic,
            IAccessControlLogic  accessControlLogic,
            bool                 isEditView
        )
        {
            DbContext            = dbContext;
            _userManagementLogic = userManagementLogic;
            _accessControlLogic  = accessControlLogic;
            _isEditView          = isEditView;
        }

        public    ILolipWikiDbContext DbContext   { get; }
        public    IRequestor          Requestor   { get; private set; }
        public    IUser               TwitchUser  { get; private set; }
        protected string              AccessToken { get; private set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            Requestor   = User.ToTwitchUser();
            AccessToken = await HttpContext.GetTokenAsync("access_token") ?? string.Empty;
            TwitchUser  = await _userManagementLogic.AddOrUpdateUserAsync(DbContext, Requestor, AccessToken);

            if (_isEditView)
                _accessControlLogic.EnsureWriteIsAllowed(DbContext, TwitchUser);
            else
                _accessControlLogic.EnsureReadIsAllowed(DbContext, TwitchUser);

            await base.OnPageHandlerExecutionAsync(context, next);
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            base.OnPageHandlerExecuted(context);

            //Documentation says DI Container creates the Instance and DI Container will Dispose it, but i dont trust the Container, sooooo
            RegisterForDispose(DbContext);
        }

        protected void RegisterForDispose(IDisposable disposable)
        {
            HttpContext.Response.RegisterForDispose(disposable);
        }
    }
}