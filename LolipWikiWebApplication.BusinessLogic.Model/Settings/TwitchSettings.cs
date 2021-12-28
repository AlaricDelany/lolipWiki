namespace LolipWikiWebApplication.BusinessLogic.Model.Settings
{
    public class TwitchSettings
    {
        public const string cSectionName = "Twitch";

        public TwitchSettings()
        {
            ClientId        = string.Empty;
            ClientSecret    = string.Empty;
            RedirectUrl     = string.Empty;
            RedirectUrlPath = string.Empty;
        }

        public TwitchSettings(
            string clientId,
            string clientSecret,
            string redirectUrl,
            string redirectUrlPath
        )
        {
            ClientId        = clientId;
            ClientSecret    = clientSecret;
            RedirectUrl     = redirectUrl;
            RedirectUrlPath = redirectUrlPath;
        }

        public string ClientId        { get; set; }
        public string ClientSecret    { get; set; }
        public string RedirectUrl     { get; set; }
        public string RedirectUrlPath { get; set; }
    }
}