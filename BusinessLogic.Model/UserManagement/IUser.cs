using System;
using System.Collections.Generic;

namespace LolipWikiWebApplication.BusinessLogic.Model.UserManagement
{
    /// <summary>
    ///     The Complete UserModel with Informations out of the Current Request, TwitchAPI and UserDB
    /// </summary>
    public interface IUser : IRequestor
    {
        public const string cRoleNameAdmin           = "AD";
        public const string cRoleNameArticleReviewer = "AR";
        public const string cRoleNameArticleManager  = "AM";
        public const string cRoleNameUserManager     = "AP";

        public static IDictionary<string, string> cUserRoleDefinitions = new Dictionary<string, string>
        {
            {
                cRoleNameAdmin, "Admin"
            },
            {
                cRoleNameArticleReviewer, "Article Reviewer"
            },
            {
                cRoleNameArticleManager, "Article Manager"
            },
            {
                cRoleNameUserManager, "User Manager"
            }
        };

        SubscriptionStateType SubscriptionState { get; }
        public DateTime?      LockedSince       { get; }
        public IUser?         LockedBy          { get; }

        bool IsAdmin           { get; }
        bool IsArticleReviewer { get; }
        bool IsArticleManager  { get; }
        bool IsUserManager     { get; }
    }
}