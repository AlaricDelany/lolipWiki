using System;
using System.Collections.Generic;
using System.Linq;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LolipWikiWebApplication.DataAccess
{
    public class LolipWikiDbContext : DbContext, ILolipWikiDbContext
    {
        public LolipWikiDbContext(DbContextOptions<LolipWikiDbContext> options) : base(options)
        {
        }

        public DbSet<UserEM>           Users           { get; set; }
        public DbSet<UserNameEM>       UserNames       { get; set; }
        public DbSet<RoleEM>           Roles           { get; set; }
        public DbSet<UserRoleEM>       UserRoles       { get; set; }
        public DbSet<ArticleEM>        Articles        { get; set; }
        public DbSet<ArticleVersionEM> ArticleVersions { get; set; }
        public DbSet<ConfigurationEM>  Configurations  { get; set; }

        DbSet<ConfigurationEM> ILolipWikiDbContext. Configurations  => Configurations;
        DbSet<UserEM> ILolipWikiDbContext.          Users           => Users;
        DbSet<UserNameEM> ILolipWikiDbContext.      UserNames       => UserNames;
        DbSet<RoleEM> ILolipWikiDbContext.          Roles           => Roles;
        DbSet<UserRoleEM> ILolipWikiDbContext.      UserRoles       => UserRoles;
        DbSet<ArticleEM> ILolipWikiDbContext.       Articles        => Articles;
        DbSet<ArticleVersionEM> ILolipWikiDbContext.ArticleVersions => ArticleVersions;

        public IDbContextTransaction BeginTransaction()
        {
            return base.Database.BeginTransaction();
        }

        public void EnsureIsUpToDate(IDictionary<string, string> roles)
        {
            Database.Migrate();

            foreach (var role in roles)
            {
                if (!Roles.Any(x => x.Name == role.Key))
                    Roles.Add(new RoleEM(role.Key, role.Value));
            }

            if (!Configurations.Any())
            {
                var defaultConfig = new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.RoleOnly);
                Configurations.Add(defaultConfig);
            }

            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //EF was unable to resolve the Many to many relation on itself so lets give it a little poosh
            modelBuilder.Entity<UserRoleEM>()
                        .HasOne(sc => sc.User)
                        .WithMany(s => s.UserRoles)
                        .HasForeignKey(sc => sc.UserId)
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<UserRoleEM>()
                        .HasOne(sc => sc.Role)
                        .WithMany(s => s.UserRoles)
                        .HasForeignKey(sc => sc.RoleId)
                        .OnDelete(DeleteBehavior.Restrict);

            // Remove all Cascade Deletes cause they only cause trouble
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                                         .SelectMany(t => t.GetForeignKeys())
                                         .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}