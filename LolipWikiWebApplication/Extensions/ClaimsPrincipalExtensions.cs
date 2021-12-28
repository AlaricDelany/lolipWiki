using System.Linq;
using System.Security.Claims;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;

namespace LolipWikiWebApplication
{
    public static class ClaimsPrincipalExtensions
    {
        private const string cEmailAddressClaimName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        private const string cUserIdClaimName       = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string cUserNameClaimName     = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private const string cDisplayNameClaimName  = "urn:twitch:displayname";
        private const string cProfileImageClaimName = "urn:twitch:profileimageurl";

        public static string GetEmail(this ClaimsPrincipal self)
        {
            return self.Claims.SingleOrDefault(x => x.Type == cEmailAddressClaimName)
                       ?.Value
                ?? string.Empty;
        }

        public static long GetUserId(this ClaimsPrincipal self)
        {
            var value = self.Claims.SingleOrDefault(x => x.Type == cUserIdClaimName)
                            ?.Value
                     ?? 0.ToString();

            return long.Parse(value);
        }

        public static string GetDisplayName(this ClaimsPrincipal self)
        {
            return self.Claims.SingleOrDefault(x => x.Type == cDisplayNameClaimName)
                       ?.Value
                ?? string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal self)
        {
            return self.Claims.SingleOrDefault(x => x.Type == cUserNameClaimName)
                       ?.Value
                ?? string.Empty;
        }

        public static string GetProfilePicture(this ClaimsPrincipal self)
        {
            return self.Claims.SingleOrDefault(x => x.Type == cProfileImageClaimName)
                       ?.Value
                ?? string.Empty;
        }

        public static IUser ToTwitchUser(this ClaimsPrincipal self)
        {
            return new UserBM(self.GetUserId(),
                                  self.GetEmail(),
                                  self.GetProfilePicture(),
                                  self.GetUserName(),
                                  self.GetDisplayName()
                                 );
        }
    }
}