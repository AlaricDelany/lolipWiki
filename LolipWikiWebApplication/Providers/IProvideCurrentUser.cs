using System.Security.Claims;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using Microsoft.AspNetCore.Http;

namespace LolipWikiWebApplication.Providers
{
    /// <summary>
    /// Should only be used at Layout.cshtml, cause it has its own DbContext
    /// </summary>
    public interface IProvideCurrentUser
    {
        IUser? GetOrDefault(HttpContext context, ClaimsPrincipal user);
    }
}