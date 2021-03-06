using System;
using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IUserRepository _userRepository;

        public ArticleRepository(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IQueryable<ArticleVersionEM> GetActiveVersions(ILolipWikiDbContext dbContext)
        {
            return dbContext.Articles.SelectMany(x => x.Versions)
                            //.Where(version => version.PublishedAt.HasValue)
                            .OrderByDescending(y => y.PublishedAt)
                            .ThenBy(y => y.Title);
            ;
        }

        public ArticleVersionEM GetVersion(ILolipWikiDbContext dbContext, long articleVersionId)
        {
            return dbContext.ArticleVersions.Single(x => x.Id == articleVersionId);
        }

        public ArticleEM Get(ILolipWikiDbContext dbContext, long articleId)
        {
            return dbContext.Articles.Single(x => x.Id == articleId);
        }

        public ArticleVersionEM Add(
            ILolipWikiDbContext dbContext,
            long                creatorId,
            string              title,
            string              imagePath
        )
        {
            var creatorUser = _userRepository.Get(dbContext, creatorId);

            var newArticle = new ArticleEM(creatorUser);
            newArticle = dbContext.Articles.Add(newArticle)
                                  .Entity;

            var articleVersion = new ArticleVersionEM(title,
                                                      "### soon™ !",
                                                      imagePath,
                                                      0,
                                                      DateTime.UtcNow,
                                                      newArticle,
                                                      creatorUser
                                                     );


            articleVersion = dbContext.ArticleVersions.Add(articleVersion)
                                      .Entity;

            return articleVersion;
        }

        public ArticleVersionEM Update(
            ILolipWikiDbContext dbContext,
            long                updaterId,
            long                articleVersionId,
            string              content
        )
        {
            var version = GetVersion(dbContext, articleVersionId);

            if (content == version.Content)
                return version;

            var updater = _userRepository.Get(dbContext, updaterId);

            version.ChangedAt   = DateTime.UtcNow;
            version.ChangedById = updater.Id;
            version.ChangedBy   = updater;
            version.Content     = content;

            return version;
        }

        public ArticleVersionEM AddVersion(ILolipWikiDbContext dbContext, ArticleVersionEM latestVersion, long creatorUserId)
        {
            var creatorUser    = _userRepository.Get(dbContext, creatorUserId);
            var articleVersion = new ArticleVersionEM(latestVersion, creatorUser.Id);

            articleVersion = dbContext.ArticleVersions.Add(articleVersion)
                                      .Entity;

            return articleVersion;
        }
    }
}