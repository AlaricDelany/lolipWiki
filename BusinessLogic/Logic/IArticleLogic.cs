using System.Collections.Generic;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public interface IArticleLogic
    {
        IEnumerable<ArticleVersionBM> GetActiveVersions(ILolipWikiDbContext dbContext, IRequestor requestor);

        IEnumerable<ArticleVersionBM> GetActiveVersions(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            int                 skip,
            int                 take,
            string              filter
        );

        ArticleVersionBM Add(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            string              title,
            string              imagePath
        );

        ArticleVersionBM Get(ILolipWikiDbContext dbContext, IRequestor requestor, long articleVersionId);

        ArticleVersionBM Update(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            long                articleVersionId,
            string              content
        );
    }
}