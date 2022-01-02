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
    public class UserManagementLogic : IUserManagementLogic
    {
        private readonly IAccessControlLogic              _accessControlLogic;
        private readonly IRoleRepository                  _roleRepository;
        private readonly IUserRepository                  _userRepository;
        private readonly ITwitchClient                    _twitchClient;
        private readonly IOptions<TwitchSettings>         _twitchSettingsOptions;
        private readonly IOptions<UserManagementSettings> _userManagementSettingsOptions;

        public UserManagementLogic(
            IOptions<TwitchSettings>         twitchSettingsOptions,
            IOptions<UserManagementSettings> userManagementSettingsOptions,
            ITwitchClient                    twitchClient,
            IUserRepository                  userRepository,
            IRoleRepository                  roleRepository,
            IAccessControlLogic              accessControlLogic
        )
        {
            _twitchSettingsOptions         = twitchSettingsOptions;
            _userManagementSettingsOptions = userManagementSettingsOptions;
            _twitchClient                  = twitchClient;
            _userRepository                = userRepository;
            _roleRepository                = roleRepository;
            _accessControlLogic            = accessControlLogic;
        }

        public IEnumerable<UserBM> GetAll(ILolipWikiDbContext dbContext, IRequestor requestor)
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var users = _userRepository.GetAll(dbContext)
                                       .ToArray();

            return users.Select(x => new UserBM(x));
        }

        public IUser GetUser(ILolipWikiDbContext dbContext, IRequestor requestor, long id)
        {
            if (requestor.Id != id)
                _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchSettings = _twitchSettingsOptions.Value;
            var user           = _userRepository.GetOrDefault(dbContext, id);

            if (user == null)
                throw new EntityNotFoundException<UserEM>(id);

            var twitchUser = new UserBM(user);

            return twitchUser;
        }

        public async Task<IUser> AddOrUpdateUserAsync(ILolipWikiDbContext dbContext, IRequestor requestor, string accessToken)
        {
            var userManagementSettings = _userManagementSettingsOptions.Value;
            var subscriptionState      = await GetSubscriptionState(accessToken, requestor.Id);

            var result = _userRepository.AddOrUpdateUser(dbContext,
                                                         requestor.Id,
                                                         requestor.UserName,
                                                         requestor.DisplayName,
                                                         requestor.Email,
                                                         requestor.ProfilePicturePath,
                                                         subscriptionState
                                                        );

            if (result.TwitchUserId == userManagementSettings.BroadcasterUserId || userManagementSettings.DefaultAdminUsers.Contains(result.TwitchUserId))
            {
                _roleRepository.Update(dbContext,
                                       result,
                                       IUser.cRoleNameAdmin,
                                       true
                                      );

                dbContext.SaveChanges();
            }

            return new UserBM(result);
        }

        public async Task<IUser> UpdateUserNameAsync(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            string              accessToken,
            long                userId
        )
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchUser = await _twitchClient.GetUserAsync(accessToken, userId);
            var user       = _userRepository.Get(dbContext, userId);


            user = _userRepository.UpdateUserName(dbContext,
                                                  user,
                                                  twitchUser.UserName,
                                                  twitchUser.DisplayName,
                                                  twitchUser.ProfilePicturePath
                                                 );
            return new UserBM(user);
        }

        public IUser ToggleLock(ILolipWikiDbContext dbContext, IRequestor requestor, long userId)
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var user = _userRepository.ToggleLock(dbContext, requestor.Id, userId);

            dbContext.SaveChanges();

            return new UserBM(user);
        }

        public async Task<IUser> ImportAsync(
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            string              accessToken,
            string              userName
        )
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var twitchUser = await _twitchClient.GetUserAsync(accessToken, userName);

            var user = _userRepository.AddOrUpdateUser(dbContext,
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
            ILolipWikiDbContext dbContext,
            IRequestor          requestor,
            long                userId,
            bool                isAdmin,
            bool                isUserManager,
            bool                isArticleManager,
            bool                isArticleReviewer
        )
        {
            _accessControlLogic.EnsureIsAllowed(dbContext, requestor, IUser.cRoleNameUserManager);

            var userToUpdate = _userRepository.Get(dbContext, userId);

            _roleRepository.Update(dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameAdmin,
                                   isAdmin
                                  );
            _roleRepository.Update(dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameUserManager,
                                   isUserManager
                                  );
            _roleRepository.Update(dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameArticleManager,
                                   isArticleManager
                                  );
            _roleRepository.Update(dbContext,
                                   userToUpdate,
                                   IUser.cRoleNameArticleReviewer,
                                   isArticleReviewer
                                  );

            dbContext.SaveChanges();

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