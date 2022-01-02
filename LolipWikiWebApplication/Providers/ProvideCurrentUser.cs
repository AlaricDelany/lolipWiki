using System;
using System.Security.Claims;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LolipWikiWebApplication.Providers
{
    public class ProvideCurrentUser : IProvideCurrentUser
    {
        private readonly ILolipWikiDbContext  _dbContext;
        private readonly ILogger              _logger;
        private readonly IUserManagementLogic _userManagementLogic;

        public ProvideCurrentUser(ILolipWikiDbContext dbContext, ILogger<ProvideCurrentUser> logger, IUserManagementLogic userManagementLogic)
        {
            _dbContext           = dbContext;
            _logger              = logger;
            _userManagementLogic = userManagementLogic;
        }

        public IUser? GetOrDefault(HttpContext context, ClaimsPrincipal user)
        {
            try
            {
                var twitchUser = user.ToTwitchUser();

                var result = _userManagementLogic.GetUser(_dbContext, twitchUser, twitchUser.Id);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.ToString());
                return null;
            }
        }
    }
}