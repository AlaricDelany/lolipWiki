using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Models;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Test.BusinessLogic.TwitchClient.Helpers
{
    /// <summary>
    /// Logic for Reading and Saving TokenModels.
    /// DO NOT COMMIT OR PUSH YOUR INTERNAL LOGIC, cause it might lead to attacks from people trying to get that Token Model
    /// </summary>
    internal static class TokenModelHelper
    {
        private const string cTokenRequestFilePath = @"D:\Temp\refresh_token_response.json";

        internal static string ReadTokenModelString()
        {
            try
            {
                return File.ReadAllText(cTokenRequestFilePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static void SaveTokenModel(TwitchTokenRefreshResponseModel model)
        {
            File.WriteAllText(cTokenRequestFilePath, JsonSerializer.Serialize(model));
        }

        internal static async Task<TwitchTokenRefreshResponseModel> GetTokenModelAsync(TwitchSettings twitchSettings)
        {
            string existingTokenModelString = ReadTokenModelString();

            if (!string.IsNullOrWhiteSpace(existingTokenModelString))
                return JsonSerializer.Deserialize<TwitchTokenRefreshResponseModel>(existingTokenModelString);

            // Generate new Token from Scratch
            string code;
            using (var driver = new FirefoxDriver())
            {
                var requestUri = $"https://id.twitch.tv/oauth2/authorize?client_id={twitchSettings.ClientId}&redirect_uri={twitchSettings.RedirectUrl}&response_type=code&scope=user:read:subscriptions";

                driver.Navigate()
                      .GoToUrl(requestUri);

                // Wait for User to do Login in the Browser
                var waitResult = new WebDriverWait(driver, TimeSpan.FromMinutes(2)).Until(drv => drv.Url.StartsWith("https://localhost"));

                Assert.IsTrue(waitResult);

                var url         = new Uri(driver.Url);
                var queryString = HttpUtility.ParseQueryString(url.Query);

                Assert.IsNotNull(queryString);

                code = queryString.Get("code");

                Assert.That(code, Is.Not.Null.Or.Empty, $"QueryString:{queryString} does not contain a Code");
            }

            using (var client = new HttpClient())
            {
                var requestUri    = $"https://id.twitch.tv/oauth2/token?client_id={twitchSettings.ClientId}&client_secret={twitchSettings.ClientSecret}&code={code}&grant_type=authorization_code&redirect_uri={twitchSettings.RedirectUrl}";
                var response      = await client.PostAsync(requestUri, new StringContent(""));
                var responseModel = await response.Content.ReadFromJsonAsync<TwitchTokenRefreshResponseModel>();

                response.EnsureSuccessStatusCode();

                Assert.That(responseModel,                Is.Not.Null);
                Assert.That(responseModel.NewAccessToken, Is.Not.Null.Or.Empty);

                TokenModelHelper.SaveTokenModel(responseModel);

                return responseModel;
            }
        }
    }
}