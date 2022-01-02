using System.Linq;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.PageModels;

namespace LolipWikiWebApplication.Pages.Article
{
    public class ArticleListModel : BasePageModel
    {
        private readonly IArticleLogic _articleLogic;

        public ArticleListModel(
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

        public ArticleVersionBM[] ArticleVersions { get; private set; }

        public void OnGet()
        {
            ArticleVersions = _articleLogic.GetActiveVersions(DbContext, Requestor)
                                           .ToArray();
        }
    }
}