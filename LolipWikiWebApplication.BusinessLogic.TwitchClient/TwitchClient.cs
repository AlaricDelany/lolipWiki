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

        public async Task<IEnumerable<TwitchSubscriptionResponseModel>> GetTwitchSubscriptionModelsAsync(string accessToken, long userId)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient(nameof(GetTwitchSubscriptionModelsAsync)))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Client-Id",     _twitchSettings.Value.ClientId);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var requestUri = $"https://api.twitch.tv/helix/subscriptions/user?user_id={userId}&broadcaster_id={_userManagementSettings.Value.BroadcasterUserId}";

                    var responseMessage = await client.GetAsync(requestUri);

                    if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                        return Array.Empty<TwitchSubscriptionResponseModel>();

                    responseMessage.EnsureSuccessStatusCode();

                    var response = await responseMessage.Content.ReadFromJsonAsync<TwitchSubscriptionsRequestModel>();

                    if (response == null)
                        throw new TwitchApiException("Twitch API returned Null");

                    return response.Data.OrderByDescending(x => x.Tier)
                                   .ToArray();
                }
            }
            catch (HttpRequestException e)
            {
                throw new TwitchApiException(e);
            }
        }

        public async Task<IRequestor> GetUserAsync(string accessToken, long userId)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient(nameof(GetUserAsync)))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Client-Id",     _twitchSettings.Value.ClientId);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var requestUri = $"https://api.twitch.tv/helix/users?id={userId}";

                    var responseMessage = await client.GetAsync(requestUri);

                    responseMessage.EnsureSuccessStatusCode();

                    var response = await responseMessage.Content.ReadFromJsonAsync<TwitchUsersRequestModel>();

                    if (response == null)
                        throw new TwitchApiException("Twitch API returned Null");

                    var result = response.Users.Single();

                    return result;
                }
            }
            catch (HttpRequestException e)
            {
                throw new TwitchApiException(e);
            }
        }

        public async Task<IRequestor> GetUserAsync(string accessToken, string userName)
        {
            try
            {
                using (var client = _httpClientFactory.CreateClient(nameof(GetUserAsync)))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Client-Id",     _twitchSettings.Value.ClientId);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var requestUri = $"https://api.twitch.tv/helix/users?login={userName}";

                    var responseMessage = await client.GetAsync(requestUri);

                    responseMessage.EnsureSuccessStatusCode();

                    var response = await responseMessage.Content.ReadFromJsonAsync<TwitchUsersRequestModel>();

                    if (response == null)
                        throw new TwitchApiException("Twitch API returned Null");

                    var result = response.Users.Single();

                    return result;
                }
            }
            catch (HttpRequestException e)
            {
                throw new TwitchApiException(e);
            }
        }
    }
}