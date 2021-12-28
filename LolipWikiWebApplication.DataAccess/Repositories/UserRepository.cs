using System;
using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserEM AddOrUpdateUser(
            ILolipWikiDbContext dbContext,
            long                userId,
            string              name,
            string              displayName,
            string              email,
            string              profilePictureImagePath,
            int                 subscriptionState
        )
        {
            var targetUser = dbContext.Users.FirstOrDefault(x => x.TwitchUserId == userId);

            if (targetUser == null)
            {
                var user = new UserEM(userId,
                                      name,
                                      displayName,
                                      email,
                                      profilePictureImagePath,
                                      subscriptionState
                                     );

                targetUser = dbContext.Users.Add(user)
                                      .Entity;
            }

            else
            {
                UpdateUserNameInternal(targetUser,
                                       name,
                                       displayName,
                                       profilePictureImagePath
                                      );
                targetUser.Email = email;
            }

            dbContext.SaveChanges();

            return targetUser;
        }

        public UserEM UpdateUserName(
            ILolipWikiDbContext dbContext,
            UserEM              targetUser,
            string              name,
            string              displayName,
            string              profilePictureImagePath
        )
        {
            UpdateUserNameInternal(targetUser,
                                   name,
                                   displayName,
                                   profilePictureImagePath
                                  );

            dbContext.SaveChanges();

            return targetUser;
        }

        /// <summary>
        ///     Needs Users and not UserIds so caller Logic can Validate if the User is Locked
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="requestorUserId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserEM ToggleLock(ILolipWikiDbContext dbContext, long requestorUserId, long userId)
        {
            var requestor = Get(dbContext, requestorUserId);
            var user      = Get(dbContext, userId);

            if (user.LockedDateTime.HasValue)
                user.LockedDateTime = null;
            else
                user.LockedDateTime = DateTime.UtcNow;

            user.LockedBy     = requestor.Id;
            user.LockedByUser = requestor;

            return user;
        }

        public IQueryable<UserEM> GetAll(ILolipWikiDbContext dbContext)
        {
            return dbContext.Users;
        }

        public UserEM Get(ILolipWikiDbContext context, long userId)
        {
            return context.Users.Single(x => x.TwitchUserId == userId);
        }

        public UserEM GetOrDefault(ILolipWikiDbContext dbContext, long userId)
        {
            return dbContext.Users.SingleOrDefault(x => x.TwitchUserId == userId);
        }

        private void UpdateUserNameInternal(
            UserEM targetUser,
            string name,
            string displayName,
            string profilePictureImagePath
        )
        {
            if (targetUser.Name != name) // Name Change
                targetUser.UserNames.Add(new UserNameEM(name,
                                                        displayName,
                                                        DateTime.UtcNow,
                                                        targetUser
                                                       )
                                        );

            targetUser.DisplayName             = displayName;
            targetUser.Name                    = name;
            targetUser.ProfilePictureImagePath = profilePictureImagePath;
        }
    }
}