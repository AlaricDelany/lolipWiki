using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_USER_ROLE")]
    public class UserRoleEM
    {
        protected UserRoleEM(long userId, long roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public UserRoleEM(UserEM user, RoleEM role) : this(user.Id, role.Id)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            User = user;
            Role = role;
            // ReSharper restore VirtualMemberCallInConstructor
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }

        public virtual UserEM User { get; set; }
        public virtual RoleEM Role { get; set; }
    }
}