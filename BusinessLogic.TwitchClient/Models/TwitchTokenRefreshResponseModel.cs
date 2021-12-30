using System.Text.Json.Serialization;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Models
{
    public record TwitchTokenRefreshResponseModel
    {
        public TwitchTokenRefreshResponseModel(string newAccessToken, int expiresIn, string refreshToken)
        {
            NewAccessToken = newAccessToken;
            ExpiresIn      = expiresIn;
            RefreshToken   = refreshToken;
        }

        [JsonPropertyName("access_token")]
        public string NewAccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}