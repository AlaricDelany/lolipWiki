using System.Collections.Generic;
using System.Linq;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementLogic _userManagementLogic;

        public UsersController(IUserManagementLogic userManagementLogic)
        {
            _userManagementLogic = userManagementLogic;
        }

        [HttpGet]
        public IEnumerable<UserBM> Get()
        {
            var requestor = User.ToTwitchUser();

            var users = _userManagementLogic.GetAll(requestor)
                                            .ToArray();

            return users;
        }
    }
}