using System;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Exceptions;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.DataAccess.Repositories;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public class AccessControlLogic : IAccessControlLogic
    {
        private readonly IUserRepository          _userRepository;
        private readonly IConfigurationRepository _configurationRepository;

        public AccessControlLogic(IUserRepository userRepository, IConfigurationRepository configurationRepository)
        {
            _userRepository          = userRepository;
            _configurationRepository = configurationRepository;
        }

        public void EnsureIsAllowed(ILolipWikiDbContext dbContext, IRequestor requestor, string roleShortName)
        {
            var user = _userRepository.Get(dbContext, requestor.Id);

            if (user.LockedDateTime.HasValue)
                throw new UserIsLockedException(requestor.Id);

            var twitchUser = new UserBM(user);

            if (twitchUser.Roles.ContainsKey(roleShortName) || twitchUser.Roles.ContainsKey(IUser.cRoleNameAdmin))
                return;

            throw new RoleNotFoundException(roleShortName, requestor.Id);
        }

        public void EnsureReadIsAllowed(ILolipWikiDbContext dbContext, IUser user)
        {
            var config = _configurationRepository.Get(dbContext);
            EnsureReadIsAllowedInternal(user, config);
        }

        private void EnsureReadIsAllowedInternal(IUser user, ConfigurationEM config)
        {
            if (user.LockedSince.HasValue)
                throw new UserIsLockedException(user.Id);

            EnsureIsAllowedInternal(user, config.ReadArticleControlTypeEnum);
        }

        public void EnsureWriteIsAllowed(ILolipWikiDbContext dbContext, IUser user)
        {
            var config = _configurationRepository.Get(dbContext);

            EnsureReadIsAllowedInternal(user, config);
            EnsureIsAllowedInternal(user, config.WriteArticleControlTypeEnum);
        }

        private void EnsureIsAllowedInternal(IUser user, AccessControlType accessControlType)
        {
            switch (accessControlType)
            {
                case AccessControlType.Everyone:
                    return;
                case AccessControlType.SubOnly:
                    if (user.SubscriptionState == SubscriptionStateType.None)
                        throw new UserSubMissingException(user.Id);
                    break;
                case AccessControlType.RoleOnly:
                    if (!(user.IsAdmin || user.IsArticleManager || user.IsArticleReviewer || user.IsUserManager))
                        throw new UserRoleMissingException(user.Id);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(accessControlType), accessControlType, null);
            }
        }
    }
}