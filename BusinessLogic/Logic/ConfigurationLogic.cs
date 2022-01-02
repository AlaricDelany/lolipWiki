using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.DataAccess.Repositories;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public class ConfigurationLogic : IConfigurationLogic
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IAccessControlLogic      _accessControlLogic;

        public ConfigurationLogic(IConfigurationRepository configurationRepository, IAccessControlLogic accessControlLogic)
        {
            _configurationRepository = configurationRepository;
            _accessControlLogic      = accessControlLogic;
        }

        public AccessControlType GetReadControlType(ILolipWikiDbContext dbContext, IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameAdmin);

            return _configurationRepository.Get(dbContext)
                                           .ReadArticleControlTypeEnum;
        }

        public AccessControlType GetWriteControlType(ILolipWikiDbContext dbContext, IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameAdmin);

            return _configurationRepository.Get(dbContext)
                                           .WriteArticleControlTypeEnum;
        }

        public void Update(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            AccessControlType   readControlType,
            AccessControlType   writeControlType
        )
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameAdmin);

            var config = _configurationRepository.Get(dbContext);

            config.ReadArticleControlTypeEnum  = readControlType;
            config.WriteArticleControlTypeEnum = writeControlType;

            dbContext.SaveChanges();
        }
    }
}