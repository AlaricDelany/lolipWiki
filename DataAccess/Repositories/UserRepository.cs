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
        /// <param name="twitchUserId"></param>
        /// <returns></returns>
        public UserEM ToggleLock(ILolipWikiDbContext dbContext, long requestorUserId, long twitchUserId)
        {
            var requestor = Get(dbContext, requestorUserId);
            var user      = Get(dbContext, twitchUserId);

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

        public UserEM Get(ILolipWikiDbContext context, long twitchUserId)
        {
            return context.Users.Single(x => x.TwitchUserId == twitchUserId);
        }

        public UserEM GetOrDefault(ILolipWikiDbContext dbContext, long twitchUserId)
        {
            return dbContext.Users.SingleOrDefault(x => x.TwitchUserId == twitchUserId);
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