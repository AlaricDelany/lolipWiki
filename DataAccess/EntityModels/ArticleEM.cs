using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_ARTICLE")]
    public class ArticleEM
    {
        protected ArticleEM(long creatorUserId)
        {
            CreatorUserId = creatorUserId;
        }

        public ArticleEM(UserEM creatorUser)
        {
            CreatorUserId = creatorUser.Id;

            // ReSharper disable VirtualMemberCallInConstructor
            CreatorUser = creatorUser;
            Versions    = new List<ArticleVersionEM>();
            // ReSharper restore VirtualMemberCallInConstructor
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(CreatorUser))]
        public long CreatorUserId { get; set; }

#region Navigation Properties

        public virtual UserEM                        CreatorUser { get; init; }
        public virtual ICollection<ArticleVersionEM> Versions    { get; init; }

#endregion Navigation Properties
    }
}