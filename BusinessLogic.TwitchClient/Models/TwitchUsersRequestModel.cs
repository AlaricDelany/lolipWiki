using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Models
{
    public record TwitchUsersRequestModel
    {
        public TwitchUsersRequestModel(IEnumerable<TwitchUserRequestModel> users)
        {
            Users = users;
        }

        [JsonPropertyName("data")]
        public IEnumerable<TwitchUserRequestModel> Users { get; set; }
    }
}