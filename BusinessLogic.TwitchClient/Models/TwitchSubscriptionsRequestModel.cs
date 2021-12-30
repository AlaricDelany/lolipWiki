using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Models
{
    public record TwitchSubscriptionsRequestModel
    {
        public TwitchSubscriptionsRequestModel(IEnumerable<TwitchSubscriptionResponseModel> data)
        {
            Data = data;
        }

        [JsonPropertyName("data")]
        public IEnumerable<TwitchSubscriptionResponseModel> Data { get; set; }
    }
}