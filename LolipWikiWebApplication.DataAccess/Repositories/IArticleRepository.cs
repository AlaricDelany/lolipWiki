using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public interface IArticleRepository
    {
        ArticleVersionEM             GetVersion(ILolipWikiDbContext        dbContext, long articleVersionId);
        IQueryable<ArticleVersionEM> GetActiveVersions(ILolipWikiDbContext dbContext);
        IQueryable<ArticleEM>        GetArticles(ILolipWikiDbContext       dbContext);

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
    }
}