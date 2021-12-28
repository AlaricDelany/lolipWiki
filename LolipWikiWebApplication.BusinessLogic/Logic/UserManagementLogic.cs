using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Exceptions;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.BusinessLogic.TwitchClient;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.Extensions.Options;

namespace LolipWikiWebApplication.BusinessLogic.Logic
{
    public class UserManagementLogic : IUserManagementLogic, IDisposable
    {
        private readonly IAccessControlLogic              _accessControlLogic;
        private readonly ILolipWikiDbContext              _dbContext;
        private readonly IRoleRepository                  _roleRepository;
        private readonly ITwitchClient                    _twitchClient;
        private readonly IOptions<TwitchSettings>         _twitchSettingsOptions;
        private readonly IOptions<UserManagementSettings> _userManagementSettingsOptions;
        private readonly IUserRepository                  _userRepository;

        public UserManagementLogic(
            IOptions<TwitchSettings>         twitchSettingsOptions,
            IOptions<UserManagementSettings> userManagementSettingsOptions,
            ITwitchClient                    twitchClient,
            IUserRepository                  userRepository,
            IRoleRepository                  roleRepository,
            IAccessControlLogic              accessControlLogic,
            ILolipWikiDbContext              dbContext
        )
        {
            _twitchSettingsOptions         = twitchSettingsOptions;
            _userManagementSettingsOptions = userManagementSettingsOptions;
            _twitchClient                  = twitchClient;
            _userRepository                = userRepository;
            _roleRepository                = roleRepository;
            _accessControlLogic            = accessControlLogic;
            _dbContext                     = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IEnumerable<UserBM> GetAll(IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);

            var users = _userRepository.GetAll(_dbContext)
                                       .ToArray();

            return users.Select(x => new UserBM(x));
        }

        public IUser GetUser(IRequestor requestor, long id)
        {
            if (requestor.Id != id)
                _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchSettings = _twitchSettingsOptions.Value;
            var user           = _userRepository.GetOrDefault(_dbContext, id);

            if (user == null)
                throw new EntityNotFoundException<UserEM>(id);

            var twitchUser = new UserBM(user);

            return twitchUser;
        }

        public async Task<IUser> AddOrUpdateUserAsync(IRequestor requestor, string accessToken)
        {
            var userManagementSettings = _userManagementSettingsOptions.Value;
            var subscriptionState      = await GetSubscriptionState(accessToken, requestor.Id);

            var result = _userRepository.AddOrUpdateUser(_dbContext,
                                                         requestor.Id,
                                                         requestor.UserName,
                                                         requestor.DisplayName,
                                                         requestor.Email,
                                                         requestor.ProfilePicturePath,
                                                         subscriptionState
                                                        );

            if (result.TwitchUserId == userManagementSettings.BroadcasterUserId || userManagementSettings.DefaultAdminUsers.Contains(result.TwitchUserId))
                _roleRepository.Update(_dbContext,
                                       result,
                                       IUser.cRoleNameAdmin,
                                       true
                                      );

            return new UserBM(result);
        }

        public async Task<IUser> UpdateUserNameAsync(IRequestor requestor, string accessToken, long userId)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchUser = await _twitchClient.GetUserAsync(accessToken, userId);
            var user       = _userRepository.Get(_dbContext, userId);


            user = _userRepository.UpdateUserName(_dbContext,
                                                  user,
                                                  twitchUser.UserName,
                                                  twitchUser.DisplayName,
                                                  twitchUser.ProfilePicturePath
                                                 );
            return new UserBM(user);
        }

        public IUser ToggleLock(IRequestor requestor, long userId)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);

            var user = _userRepository.ToggleLock(_dbContext, requestor.Id, userId);

            _dbContext.SaveChanges();

            return new UserBM(user);
        }

        public async Task<IUser> ImportAsync(IRequestor requestor, string accessToken, string userName)
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchUser = await _twitchClient.GetUserAsync(accessToken, userName);

            var user = _userRepository.AddOrUpdateUser(_dbContext,
                                                       twitchUser.Id,
                                                       twitchUser.UserName,
                                                       twitchUser.DisplayName,
                                                       string.Empty,
                                                       twitchUser.ProfilePicturePath,
                                                       (int) SubscriptionStateType.None
                                                      );
            return new UserBM(user);
        }

        public IUser UpdateRoles(
            IRequestor requestor,
            long       userId,
            bool       isAdmin,
            bool       isUserManager,
            bool       isArticleManager,
            bool       isArticleReviewer
        )
        {
            _accessControlLogic.EnsureIsAllowed(_dbContext, requestor, IUser.cRoleNameUserManager);


            var userToUpdate = _userRepository.Get(_dbContext, userId);

            _roleRepository.Update(_dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameAdmin,
                                   isAdmin
                                  );
            _roleRepository.Update(_dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameUserManager,
                                   isUserManager
                                  );
            _roleRepository.Update(_dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameArticleManager,
                                   isArticleManager
                                  );
            _roleRepository.Update(_dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameArticleReviewer,
                                   isArticleReviewer
                                  );

            return new UserBM(userToUpdate);
        }

        private async Task<int> GetSubscriptionState(string accessToken, long userId)
        {
            var subscriptionModels = await _twitchClient.GetTwitchSubscriptionModelsAsync(accessToken, userId);
            var subscriptionTiers = subscriptionModels.Select(x => x.Tier)
                                                      .ToArray();

            if (!subscriptionTiers.Any())
                return (int) SubscriptionStateType.None;

            return subscriptionTiers.Max();
        }
    }
}