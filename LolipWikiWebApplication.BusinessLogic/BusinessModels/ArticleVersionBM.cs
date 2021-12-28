using System;
using System.ComponentModel.DataAnnotations;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.BusinessLogic.BusinessModels
{
    public record ArticleVersionBM
    {
        public ArticleVersionBM(ArticleVersionEM articleVersion)
        {
            Id          = articleVersion.Id;
            ArticleId   = articleVersion.ArticleId;
            Title       = articleVersion.Title;
            Content     = articleVersion.Content;
            TitleImage  = articleVersion.TitleImage;
            Revision    = articleVersion.Revision;
            CreatedDate = articleVersion.CreatedDate;
            ChangedAt   = articleVersion.ChangedAt;
            PublishedAt = articleVersion.PublishedAt;
            ChangedBy   = articleVersion.ChangedBy == null ? null : new UserBM(articleVersion.ChangedBy);
            CreatedBy   = new UserBM(articleVersion.CreatedBy);
        }

        [Required]
        public long Id { get; set; }

        [Required]
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

        public IRequestor? ChangedBy { get; set; }
        public IRequestor  CreatedBy { get; set; }
    }
}