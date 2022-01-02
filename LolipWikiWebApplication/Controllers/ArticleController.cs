using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ILolipWikiDbContext _dbContext;
        private readonly IArticleLogic       _articleLogic;

        public ArticleController(ILolipWikiDbContext dbContext, IArticleLogic articleLogic)
        {
            _dbContext    = dbContext;
            _articleLogic = articleLogic;
        }

        [HttpGet]
        public IEnumerable<ArticleVersionBM> Get(int skip, int take, [FromQuery] string filter)
        {
            var requestor = User.ToTwitchUser();

            if (string.IsNullOrWhiteSpace(filter))
                return _articleLogic.GetActiveVersions(_dbContext, requestor);

            //Have to remove the Additional Braces cause stupid DevExpress added multiple of them so its not a Valid String[] anymore, only God knows why. even a string[][] would need curly braces and not the standard one, really weird
            var substring    = filter.Substring(1, filter.Length - 2);
            var searchString = JsonSerializer.Deserialize<string[]>(substring);
            var result = _articleLogic.GetActiveVersions(_dbContext,
                                                         requestor,
                                                         skip,
                                                         take,
                                                         searchString.Last()
                                                        )
                                      .ToArray();

            return result;
        }

        [HttpPost]
        public HttpStatusCode OnPost([FromQuery] long articleId, [FromForm] string content)
        {
            return HttpStatusCode.OK;
        }
    }
}