using System.Text.Json.Serialization;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Models
{
    public record TwitchUserRequestModel : IRequestor
    {
        public TwitchUserRequestModel(
            long   id,
            string email,
            string profilePicturePath,
            string userName,
            string displayName
        )
        {
            Id                 = id;
            Email              = email;
            ProfilePicturePath = profilePicturePath;
            UserName           = userName;
            DisplayName        = displayName;
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string ProfilePicturePath { get; set; }

        [JsonPropertyName("login")]
        public string UserName { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }
}