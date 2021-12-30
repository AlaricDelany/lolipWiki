using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_ROLE")]
    public class RoleEM
    {
        public RoleEM(string name, string displayName)
        {
            Name        = name;
            DisplayName = displayName;

            // ReSharper disable once VirtualMemberCallInConstructor
            UserRoles = new List<UserRoleEM>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Name { get; set; }

        [Required]
        [StringLength(256)]
        public string DisplayName { get; set; }

#region Navigation Properties

        public virtual ICollection<UserRoleEM> UserRoles { get; set; }

#endregion Navigation Properties
    }
}