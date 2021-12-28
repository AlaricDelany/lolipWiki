using System;

namespace LolipWikiWebApplication.BusinessLogic.Model.Settings
{
    public class UserManagementSettings
    {
        public const string cSectionName = "UserManagement";

        public UserManagementSettings()
        {
            BroadcasterUserId = -1;
            DefaultAdminUsers = Array.Empty<long>();
        }

        public UserManagementSettings(long broadcasterUserId, long[] defaultAdminUsers)
        {
            BroadcasterUserId = broadcasterUserId;
            DefaultAdminUsers = defaultAdminUsers;
        }

        public long   BroadcasterUserId { get; set; }
        public long[] DefaultAdminUsers { get; set; }
    }
}