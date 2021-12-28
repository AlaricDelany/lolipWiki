using System.Text.Json.Serialization;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Models
{
    public record TwitchSubscriptionResponseModel
    {
        public TwitchSubscriptionResponseModel(
            string broadcasterId,
            string broadcasterDisplayName,
            bool   isGiftSub,
            int    tier
        )
        {
            BroadcasterId          = broadcasterId;
            BroadcasterDisplayName = broadcasterDisplayName;
            IsGiftSub              = isGiftSub;
            Tier                   = tier;
        }

        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }

        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterDisplayName { get; set; }

        [JsonPropertyName("is_gift")]
        public bool IsGiftSub { get; set; }

        [JsonPropertyName("tier")]
        public int Tier { get; set; }
    }
}