using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;

namespace LolipWikiWebApplication.DataAccess.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public void Update(
            ILolipWikiDbContext dbContext,
            UserEM              user,
            string              roleShortName,
            bool                shouldHaveRole
        )
        {
            var role = Get(dbContext, roleShortName);

            if (shouldHaveRole)
            {
                if (user.UserRoles.Any(x => x.RoleId == role.Id))
                    return;

                user.UserRoles.Add(new UserRoleEM(user, role));
            }
            else
            {
                var userRole = user.UserRoles.FirstOrDefault(x => x.RoleId == role.Id);

                if (userRole != null)
                    dbContext.UserRoles.Remove(userRole);
            }

            dbContext.SaveChanges();
        }

        public RoleEM Get(ILolipWikiDbContext lolipWikiDbContext, string roleShortName)
        {
            var role = lolipWikiDbContext.Roles.Single(x => x.Name == roleShortName);

            return role;
        }
    }
}