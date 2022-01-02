using System.ComponentModel.DataAnnotations;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.Article
{
    public class AddArticleModel : BasePageModel
    {
        private readonly IArticleLogic _articleLogic;

        public AddArticleModel(
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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var article = _articleLogic.Add(DbContext,
                                            Requestor,
                                            Title,
                                            TitleImage
                                           );

            return RedirectToPage("Detail",
                                  new
                                  {
                                      articleId        = article.ArticleId,
                                      articleVersionId = article.Id
                                  }
                                 );
        }
    }
}