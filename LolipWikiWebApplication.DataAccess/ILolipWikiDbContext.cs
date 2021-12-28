using System;
using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LolipWikiWebApplication.DataAccess
{
    public interface ILolipWikiDbContext : IDisposable
    {
        internal DbSet<UserEM>           Users           { get; }
        internal DbSet<UserNameEM>       UserNames       { get; }
        internal DbSet<RoleEM>           Roles           { get; }
        internal DbSet<UserRoleEM>       UserRoles       { get; }
        internal DbSet<ArticleEM>        Articles        { get; }
        internal DbSet<ArticleVersionEM> ArticleVersions { get; }
        internal DbSet<ConfigurationEM>  Configurations  { get; }

        int                   SaveChanges();
        IDbContextTransaction BeginTransaction();
    }
}