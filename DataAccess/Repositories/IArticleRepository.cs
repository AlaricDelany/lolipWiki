using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public interface IArticleRepository
    {
        ArticleEM                    Get(ILolipWikiDbContext               dbContext, long articleId);
        ArticleVersionEM             GetVersion(ILolipWikiDbContext        dbContext, long articleVersionId);
        IQueryable<ArticleVersionEM> GetActiveVersions(ILolipWikiDbContext dbContext);

        ArticleVersionEM Add(
            ILolipWikiDbContext dbContext,
            long                creatorId,
            string              title,
            string              imagePath
        );

        ArticleVersionEM Update(
            ILolipWikiDbContext dbContext,
            long                updaterId,
            long                articleVersionId,
            string              content
        );

        ArticleVersionEM AddVersion(ILolipWikiDbContext dbContext, ArticleVersionEM latestVersion, long creatorUserId);
    }
}