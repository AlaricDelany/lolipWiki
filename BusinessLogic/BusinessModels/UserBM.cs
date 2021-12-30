using System;
using System.Collections.Generic;
using System.Linq;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.BusinessLogic.BusinessModels
{
    public record UserBM : IUser
    {
        public UserBM(
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
            SubscriptionState  = SubscriptionStateType.None;
            Roles              = new Dictionary<string, string>();
        }

        public UserBM(IRequestor requestor) : this(requestor.Id,
                                                   requestor.Email,
                                                   requestor.ProfilePicturePath,
                                                   requestor.UserName,
                                                   requestor.DisplayName
                                                  )
        {
        }

        public UserBM(UserEM user) : this(user.TwitchUserId,
                                          user.Email,
                                          user.ProfilePictureImagePath,
                                          user.Name,
                                          user.DisplayName
                                         )
        {
            Roles       = user.UserRoles.ToDictionary(x => x.Role.Name, x => x.Role.DisplayName);
            LockedSince = user.LockedDateTime;
            LockedBy    = user.LockedByUser == null ? null : new UserBM(user.LockedByUser);

            switch (user.SubscriptionState)
            {
                case 1000:
                    SubscriptionState = SubscriptionStateType.T1;
                    break;
                case 2000:
                    SubscriptionState = SubscriptionStateType.T2;
                    break;
                case 3000:
                    SubscriptionState = SubscriptionStateType.T3;
                    break;
                default:
                    SubscriptionState = SubscriptionStateType.None;
                    break;
            }
        }

        public IDictionary<string, string> Roles { get; internal set; }

        public long                  Id                 { get; }
        public string                Email              { get; }
        public string                ProfilePicturePath { get; }
        public string                UserName           { get; }
        public string                DisplayName        { get; }
        public SubscriptionStateType SubscriptionState  { get; internal set; }
        public DateTime?             LockedSince        { get; set; }
        public UserBM?               LockedBy           { get; set; }
        IUser? IUser.                LockedBy           => LockedBy;

#region calculated

        public bool IsAdmin           => Roles.ContainsKey(IUser.cRoleNameAdmin);
        public bool IsArticleReviewer => IsAdmin || Roles.ContainsKey(IUser.cRoleNameArticleReviewer);
        public bool IsArticleManager  => IsAdmin || Roles.ContainsKey(IUser.cRoleNameArticleManager);
        public bool IsUserManager     => IsAdmin || Roles.ContainsKey(IUser.cRoleNameUserManager);

#endregion calculated
    }
}