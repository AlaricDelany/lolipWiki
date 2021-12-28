using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.DataAccess.Repositories;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public class ConfigurationLogic : IConfigurationLogic
    {
        private readonly ILolipWikiDbContext      _dbContext;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IAccessControlLogic      _accessControlLogic;

        public ConfigurationLogic(ILolipWikiDbContext dbContext, IConfigurationRepository configurationRepository, IAccessControlLogic accessControlLogic)
        {
            _dbContext               = dbContext;
            _configurationRepository = configurationRepository;
            _accessControlLogic      = accessControlLogic;
        }

        public AccessControlType GetReadControlType(IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameAdmin);

            return _configurationRepository.Get(_dbContext)
                                           .ReadArticleControlTypeEnum;
        }

        public AccessControlType GetWriteControlType(IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameAdmin);

            return _configurationRepository.Get(_dbContext)
                                           .WriteArticleControlTypeEnum;
        }

        public void Update(IRequestor requestor, AccessControlType readControlType, AccessControlType writeControlType)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameAdmin);

            var config = _configurationRepository.Get(_dbContext);

            config.ReadArticleControlTypeEnum  = readControlType;
            config.WriteArticleControlTypeEnum = writeControlType;

            _dbContext.SaveChanges();
        }
    }
}