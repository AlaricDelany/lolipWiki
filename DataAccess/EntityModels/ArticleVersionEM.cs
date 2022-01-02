using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_ARTICLE_VERSION")]
    public class ArticleVersionEM
    {
        public ArticleVersionEM(ArticleVersionEM latestVersion, long createdById) : this(latestVersion.ArticleId,
                                                                                         latestVersion.Title,
                                                                                         latestVersion.Content,
                                                                                         latestVersion.TitleImage,
                                                                                         latestVersion.Revision++,
                                                                                         DateTime.UtcNow,
                                                                                         createdById
                                                                                        )
        {
        }

        protected ArticleVersionEM(
            long     articleId,
            string   title,
            string   content,
            string   titleImage,
            int      revision,
            DateTime createdDate,
            long     createdById
        )
        {
            ArticleId   = articleId;
            Title       = title;
            Content     = content;
            TitleImage  = titleImage;
            Revision    = revision;
            CreatedDate = createdDate;
            CreatedById = createdById;
        }

        public ArticleVersionEM(
            string    title,
            string    content,
            string    titleImage,
            int       revision,
            DateTime  createdDate,
            ArticleEM article,
            UserEM    createdBy
        ) : this(article.Id,
                 title,
                 content,
                 titleImage,
                 revision,
                 createdDate,
                 createdBy.Id
                )
        {
            // ReSharper disable VirtualMemberCallInConstructor
            Article   = article;
            CreatedBy = createdBy;
            // ReSharper restore VirtualMemberCallInConstructor
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Article))]
        public long ArticleId { get; set; }

        [Required]
        [StringLength(byte.MaxValue)]
        public string Title { get; set; }

        [StringLength(int.MaxValue)]
        public string Content { get; set; }

        [StringLength(1024)]
        public string TitleImage { get; set; }

        [Required]
        public int Revision { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedAt   { get; set; }
        public DateTime? PublishedAt { get; set; }

        [ForeignKey(nameof(ChangedBy))]
        public long? ChangedById { get; set; }

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        public long CreatedById { get; set; }

#region Navigation Properties

        public virtual ArticleEM Article   { get; set; }
        public virtual UserEM    ChangedBy { get; set; }
        public virtual UserEM    CreatedBy { get; set; }

#endregion Navigation Properties
    }
}