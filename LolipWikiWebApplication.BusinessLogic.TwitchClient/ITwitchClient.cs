using System.Collections.Generic;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Models;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient
{
    public interface ITwitchClient
    {
        Task<IRequestor>                                   GetUserAsync(string                     accessToken);
        Task<IRequestor>                                   GetUserAsync(string                     accessToken, long   userId);
        Task<IRequestor>                                   GetUserAsync(string                     accessToken, string userName);
        Task<TwitchTokenRefreshResponseModel>              RefreshTokenAsync(string                accessToken, string refreshToken);
        Task<IEnumerable<TwitchSubscriptionResponseModel>> GetTwitchSubscriptionModelsAsync(string accessToken, long   userId);
    }
}