using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_USERNAME")]
    public class UserNameEM
    {
        protected UserNameEM(
            string   name,
            string   displayName,
            DateTime archivedDate,
            long     userId
        )
        {
            Name         = name;
            DisplayName  = displayName;
            ArchivedDate = archivedDate;
            UserId       = userId;
        }

        public UserNameEM(
            string   name,
            string   displayName,
            DateTime archivedDate,
            UserEM   user
        ) : this(name,
                 displayName,
                 archivedDate,
                 user.Id
                )
        {
            // ReSharper disable VirtualMemberCallInConstructor
            User = user;
            // ReSharper restore VirtualMemberCallInConstructor
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }

        [Required]
        public DateTime ArchivedDate { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public virtual UserEM User { get; set; }
    }
}