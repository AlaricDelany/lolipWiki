using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_USER")]
    public class UserEM
    {
        public UserEM(
            long   twitchUserId,
            string name,
            string displayName,
            string email,
            string profilePictureImagePath,
            int    subscriptionState
        )
        {
            TwitchUserId            = twitchUserId;
            Name                    = name;
            DisplayName             = displayName;
            Email                   = email;
            ProfilePictureImagePath = profilePictureImagePath;
            SubscriptionState       = subscriptionState;

            // ReSharper disable VirtualMemberCallInConstructor
            UserNames = new List<UserNameEM>();
            UserRoles = new List<UserRoleEM>();
            // ReSharper restore VirtualMemberCallInConstructor
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long TwitchUserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(2048)]
        public string ProfilePictureImagePath { get; set; }

        [Required]
        public int SubscriptionState { get; set; }

        public DateTime? LockedDateTime { get; set; }

        [ForeignKey(nameof(LockedByUser))]
        public long? LockedBy { get; set; }

#region Navigation Properties

        public virtual UserEM LockedByUser { get; set; }

        [InverseProperty(nameof(UserNameEM.User))]
        public virtual ICollection<UserNameEM> UserNames { get; init; }

        [InverseProperty(nameof(UserRoleEM.User))]
        public virtual ICollection<UserRoleEM> UserRoles { get; init; }

#endregion Navigation Properties
    }
}