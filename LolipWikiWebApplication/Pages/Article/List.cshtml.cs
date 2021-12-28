using System.Linq;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.PageModels;

namespace LolipWikiWebApplication.Pages.Article
{
    public class ArticleListModel : BasePageModel
    {
        private readonly IArticleLogic _articleLogic;

        public ArticleListModel(IUserManagementLogic userManagementLogic, IArticleLogic articleLogic, IAccessControlLogic accessControlLogic) : base(userManagementLogic, accessControlLogic, false)
        {
            _articleLogic = articleLogic;
        }

        public ArticleVersionBM[] ArticleVersions { get; private set; }

        public void OnGet()
        {
            ArticleVersions = _articleLogic.GetActiveVersions(Requestor)
                                           .ToArray();
        }
    }
}