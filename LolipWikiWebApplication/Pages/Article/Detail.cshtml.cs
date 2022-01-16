using System.ComponentModel.DataAnnotations;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.Article
{
    public class ArticleDetailModel : BasePageModel
    {
        private readonly IArticleLogic _articleLogic;

        public ArticleDetailModel(
            ILolipWikiDbContext  dbContext,
            IUserManagementLogic userManagementLogic,
            IArticleLogic        articleLogic,
            IAccessControlLogic  accessControlLogic
        ) : base(dbContext,
                 userManagementLogic,
                 accessControlLogic,
                 true
                )
        {
            _articleLogic = articleLogic;
        }

        [Required]
        [StringLength(byte.MaxValue)]
        [BindProperty]
        public string Title { get; set; }

        [Required]
        [StringLength(1024)]
        [BindProperty]
        public string TitleImage { get; set; }

        [Required]
        [BindProperty]
        public string ArticleContent { get; set; }

        public ArticleVersionBM Version { get; set; }

        public void OnGet(long articleId, long articleVersionId)
        {
            var article = _articleLogic.Get(DbContext, Requestor, articleVersionId);

            Version        = article;
            Title          = article.Title;
            TitleImage     = article.TitleImage;
            ArticleContent = article.Content;
        }

        public IActionResult OnPost(long articleId, long articleVersionId)
        {
            var article = _articleLogic.Update(DbContext,
                                               Requestor,
                                               articleVersionId,
                                               ArticleContent
                                              );

            return RedirectToPage("List");
        }
    }
}