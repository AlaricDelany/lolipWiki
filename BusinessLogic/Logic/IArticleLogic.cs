using System.Collections.Generic;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IArticleLogic
    {
        IEnumerable<ArticleVersionBM> GetActiveVersions(IRequestor requestor);

        IEnumerable<ArticleVersionBM> GetActiveVersions(
            IRequestor requestor,
            int        skip,
            int        take,
            string     filter
        );

        ArticleVersionBM Add(IRequestor    requestor, string title, string imagePath);
        ArticleVersionBM Get(IRequestor    requestor, long   articleVersionId);
        ArticleVersionBM Update(IRequestor requestor, long   articleVersionId, string content);
    }
}