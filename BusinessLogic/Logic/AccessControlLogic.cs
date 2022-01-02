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

        public void EnsureReadIsAllowedInternal(IUser user, ConfigurationEM config)
        {
            if (user.LockedSince.HasValue)
                throw new UnauthorizedAccessException($"User:{user.Id} is locked.");

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
                        throw new UnauthorizedAccessException($"User:{user.Id} is not subbed.");
                    break;
                case AccessControlType.RoleOnly:
                    if (!(user.IsAdmin || user.IsArticleManager || user.IsArticleReviewer || user.IsUserManager))
                        throw new UnauthorizedAccessException($"User:{user.Id} has no Roles.");

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(accessControlType), accessControlType, null);
            }
        }
    }
}