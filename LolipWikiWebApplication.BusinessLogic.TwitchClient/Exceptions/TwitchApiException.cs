using System;
using System.Diagnostics.CodeAnalysis;
using LolipWikiWebApplication.BusinessLogic.Model;
using LolipWikiWebApplication.BusinessLogic.Model.Exceptions;

namespace LolipWikiWebApplication.BusinessLogic.TwitchClient.Exceptions
{
    [SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>")]
    public class TwitchApiException : BusinessLayerException
    {
        public TwitchApiException(Exception innerException) : base(BusinessLayerErrorCode.ApiException, "Unknown Error while calling the Twitch API", innerException)
        {
        }

        public TwitchApiException(string message) : base(BusinessLayerErrorCode.ApiException, message)
        {
        }
    }
}