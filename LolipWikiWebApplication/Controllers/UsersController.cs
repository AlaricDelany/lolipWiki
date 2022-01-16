using System.Collections.Generic;
using System.Linq;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILolipWikiDbContext  _dbContext;
        private readonly IUserManagementLogic _userManagementLogic;

        public UsersController(ILolipWikiDbContext dbContext, IUserManagementLogic userManagementLogic)
        {
            _dbContext           = dbContext;
            _userManagementLogic = userManagementLogic;
        }

        [HttpGet]
        public IEnumerable<UserBM> Get()
        {
            var requestor = User.ToTwitchUser();

            var users = _userManagementLogic.GetAll(_dbContext, requestor)
                                            .ToArray();

            return users;
        }
    }
}