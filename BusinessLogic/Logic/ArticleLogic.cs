using System;
using System.Collections.Generic;
using System.Linq;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Exceptions;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Repositories;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public class ArticleLogic : IArticleLogic
    {
        private readonly IAccessControlLogic _accessControlLogic;
        private readonly IArticleRepository  _articleRepository;
        private readonly ILolipWikiDbContext _dbContext;

        public ArticleLogic(ILolipWikiDbContext dbContext, IAccessControlLogic accessControlLogic, IArticleRepository articleRepository)
        {
            _dbContext          = dbContext;
            _accessControlLogic = accessControlLogic;
            _articleRepository  = articleRepository;
        }

        public IEnumerable<ArticleVersionBM> GetActiveVersions(IRequestor requestor)
        {
            var articleVersions = _articleRepository.GetActiveVersions(_dbContext)
                                                    .OrderBy(x => x.Title)
                                                    .ToArray();

            return articleVersions.Select(x => new ArticleVersionBM(x));
        }

        public IEnumerable<ArticleVersionBM> GetActiveVersions(
            IRequestor requestor,
            int        skip,
            int        take,
            string     filter
        )
        {
            var safeFilter = filter; //Remove/Escape Characters you do not want here
            var articleVersions = _articleRepository.GetActiveVersions(_dbContext)
                                                    .Where(x => x.Title.Contains(safeFilter));

            return articleVersions.OrderBy(x => x.Title)
                                  .Skip(skip)
                                  .Take(take)
                                  .ToArray()
                                  .Select(x => new ArticleVersionBM(x));
        }

        public ArticleVersionBM Get(IRequestor requestor, long articleVersionId)
        {
            var articleVersion = _articleRepository.GetVersion(_dbContext, articleVersionId);

            return new ArticleVersionBM(articleVersion);
        }

        public ArticleVersionBM Add(IRequestor requestor, string title, string imagePath)
        {
            var articleVersion = _articleRepository.Add(_dbContext,
                                                        requestor.Id,
                                                        title,
                                                        imagePath
                                                       );

            _dbContext.SaveChanges();

            return new ArticleVersionBM(articleVersion);
        }

        public ArticleVersionBM Update(IRequestor requestor, long articleVersionId, string content)
        {
            var articleVersion = _articleRepository.Update(_dbContext,
                                                           requestor.Id,
                                                           articleVersionId,
                                                           content
                                                          );

            _dbContext.SaveChanges();

            return new ArticleVersionBM(articleVersion);
        }

        public ArticleVersionEM AddVersion(ILolipWikiDbContext dbContext, IRequestor requestor, long articleId)
        {
            var article = _articleRepository.Get(dbContext, articleId);
            var versions = article.Versions.OrderByDescending(x => x.Revision)
                                  .ToArray();
            var latestVersion = versions.First();

            if (!latestVersion.PublishedAt.HasValue)
                throw new ExistingDraftException(articleId, latestVersion.Id, requestor.Id);

            var newVersion = _articleRepository.AddVersion(dbContext, latestVersion, requestor.Id);

            return newVersion;
        }
    }
}