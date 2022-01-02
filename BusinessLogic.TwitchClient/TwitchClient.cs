using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Exceptions;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Models;
using Microsoft.Extensions.Options;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient
{
    public class TwitchClient : ITwitchClient
    {
        private readonly IHttpClientFactory               _httpClientFactory;
        private readonly IOptions<TwitchSettings>         _twitchSettings;
        private readonly IOptions<UserManagementSettings> _userManagementSettings;

        public TwitchClient(IHttpClientFactory httpClientFactory, IOptions<UserManagementSettings> userManagementSettings, IOptions<TwitchSettings> twitchSettings)
        {
            _httpClientFactory      = httpClientFactory;
            _userManagementSettings = userManagementSettings;
            _twitchSettings         = twitchSettings;
        }

        private async Task<TResult?> ExecuteWithClientAsync<TResult>(string accessToken, Func<HttpClient, Task<HttpResponseMessage>> httpOperation, HttpStatusCode[] statusCodeWhiteList) where TResult : class
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Client-Id",     _twitchSettings.Value.ClientId);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var responseMessage = await httpOperation(client);

                    if (!statusCodeWhiteList.Contains(responseMessage.StatusCode))
                        responseMessage.EnsureSuccessStatusCode();
                    else
                        return null;

                    var response = await responseMessage.Content.ReadFromJsonAsync<TResult>();

                    if (response == null)
                        throw new TwitchApiException("Twitch API returned Null");

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                throw new TwitchApiException(e);
            }
        }

        private async Task<TResult?> GetAsync<TResult>(string accessToken, string requestUri, params HttpStatusCode[] statusCodeWhiteList) where TResult : class
        {
            return await ExecuteWithClientAsync<TResult>(accessToken, async client => await client.GetAsync(requestUri), statusCodeWhiteList);
        }

        private async Task<TResult?> PostAsync<TResult>(string accessToken, string requestUri, params HttpStatusCode[] statusCodeWhiteList) where TResult : class
        {
            return await ExecuteWithClientAsync<TResult>(accessToken, async client => await client.PostAsync(requestUri, new StringContent("")), statusCodeWhiteList);
        }

        public async Task<IEnumerable<TwitchSubscriptionResponseModel>> GetTwitchSubscriptionModelsAsync(string accessToken, long userId)
        {
            var response = await GetAsync<TwitchSubscriptionsRequestModel>(accessToken, $"https://api.twitch.tv/helix/subscriptions/user?user_id={userId}&broadcaster_id={_userManagementSettings.Value.BroadcasterUserId}", HttpStatusCode.NotFound);

            if (response == null)
                return Array.Empty<TwitchSubscriptionResponseModel>();

            return response.Data.OrderByDescending(x => x.Tier)
                           .ToArray();
        }

        public async Task<IRequestor> GetUserAsync(string accessToken, long userId)
        {
            var response = await GetAsync<TwitchUsersRequestModel>(accessToken, $"https://api.twitch.tv/helix/users?id={userId}");
            var result   = response.Users.Single();

            return result;
        }

        public async Task<IRequestor> GetUserAsync(string accessToken)
        {
            var response = await GetAsync<TwitchUsersRequestModel>(accessToken, "https://api.twitch.tv/helix/users");
            var result   = response.Users.Single();

            return result;
        }

        public async Task<IRequestor> GetUserAsync(string accessToken, string userName)
        {
            var response = await GetAsync<TwitchUsersRequestModel>(accessToken, $"https://api.twitch.tv/helix/users?login={userName}");
            var result   = response.Users.Single();

            return result;
        }

        public async Task<TwitchTokenRefreshResponseModel> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var response = await PostAsync<TwitchTokenRefreshResponseModel>(accessToken, $"https://id.twitch.tv/oauth2/token?grant_type=refresh_token&refresh_token={refreshToken}&client_id={_twitchSettings.Value.ClientId}&client_secret={_twitchSettings.Value.ClientSecret}");

            return response;
        }
    }
}