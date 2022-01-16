using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;

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
                 false
                )
        {
            _articleLogic = articleLogic;
        }

        public ArticleVersionBM Version { get; set; }

        public void OnGet(long articleId, long articleVersionId)
        {
            var article = _articleLogic.Get(DbContext, Requestor, articleVersionId);

            Version = article;
        }
    }
}