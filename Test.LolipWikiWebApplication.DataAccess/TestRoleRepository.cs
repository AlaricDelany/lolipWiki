using System;
using System.Linq;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Test.LolipWikiWebApplication.DataAccess
{
    public class TestRoleRepository
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IRoleRepository  Repository      { get; set; }

        [SetUp]
        public void Setup()
        {
            void SetupServiceCollection()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddDbContext<ILolipWikiDbContext, LolipWikiDbContext>(options =>
                                                                                        {
                                                                                            options.UseInMemoryDatabase(Guid.NewGuid()
                                                                                                                            .ToString("N")
                                                                                                                       );
                                                                                        }
                                                                                       );
                serviceCollection.AddRepositories();

                ServiceProvider = serviceCollection.BuildServiceProvider();
            }

            SetupServiceCollection();

            Repository = ServiceProvider.GetService<IRoleRepository>();
        }

        [Test]
        public void Test_Get_ThrowsIfRoleNotFound()
        {
            const string cRoleName = "Test";

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var role = Repository.Get(context, cRoleName);
                                                         }
                                                        );

                context.Roles.Add(new RoleEM(cRoleName, cRoleName));
                context.SaveChanges();

                var role = Repository.Get(context, cRoleName);

                Assert.That(role,      Is.Not.Null);
                Assert.That(role.Name, Is.EqualTo(cRoleName));
            }
        }

        [Test]
        public void Test_Update_AddRoleWorks()
        {
            const string cRoleName = "Test";

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var roleEM = new RoleEM(cRoleName, cRoleName);
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Roles.Add(roleEM);
                context.Users.Add(userEM);
                context.SaveChanges();

                Repository.Update(context, userEM, cRoleName, true);
                context.SaveChanges();

                Assert.That(userEM,                         Is.Not.Null);
                Assert.That(userEM.UserRoles,               Is.Not.Empty);
                Assert.That(userEM.UserRoles.Single().Role, Is.EqualTo(roleEM));
            }
        }

        [Test]
        public void Test_Update_RemoveRoleWorks()
        {
            const string cRoleName = "Test";

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var roleEM = new RoleEM(cRoleName, cRoleName);
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Roles.Add(roleEM);
                context.Users.Add(userEM);
                context.SaveChanges();

                Repository.Update(context, userEM, cRoleName, true);
                context.SaveChanges();

                Assert.That(userEM,                         Is.Not.Null);
                Assert.That(userEM.UserRoles,               Is.Not.Empty);
                Assert.That(userEM.UserRoles.Single().Role, Is.EqualTo(roleEM));


                Repository.Update(context, userEM, cRoleName, false);
                context.SaveChanges();

                Assert.That(userEM,                         Is.Not.Null);
                Assert.That(userEM.UserRoles,               Is.Empty);
            }
        }
    }
}