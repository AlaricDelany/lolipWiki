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
    public class TestUserRepository
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IUserRepository  Repository      { get; set; }

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

            Repository = ServiceProvider.GetService<IUserRepository>();
        }

        [Test]
        public void Test_UpdateUserName_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                const string newName = "newName";
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);
                context.SaveChanges();
                Assert.That(userEM.UserNames, Is.Empty);


                //Act
                var updatedUser = Repository.UpdateUserName(context,
                                                            userEM,
                                                            newName,
                                                            newName,
                                                            ""
                                                           );

                Assert.That(updatedUser,           Is.Not.Null);
                Assert.That(updatedUser.UserNames, Is.Not.Null.Or.Empty);
                Assert.That(updatedUser.Name,      Is.EqualTo(newName));
            }
        }

        [Test]
        public void Test_ToggleLock_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var user1 = new UserEM(42,
                                       "Name1",
                                       "Name1",
                                       "",
                                       "",
                                       0
                                      );

                var user2 = new UserEM(43,
                                       "Name2",
                                       "Name2",
                                       "",
                                       "",
                                       0
                                      );
                context.Users.Add(user1);
                context.Users.Add(user2);
                context.SaveChanges();


                //Act
                var updatedUser = Repository.ToggleLock(context, user1.TwitchUserId, user2.TwitchUserId);

                Assert.That(updatedUser,                Is.Not.Null);
                Assert.That(updatedUser.LockedDateTime, Is.Not.Null);
                Assert.That(updatedUser.LockedBy,       Is.Not.Null);
                Assert.That(updatedUser.LockedByUser,   Is.EqualTo(user1));

                updatedUser = Repository.ToggleLock(context, user1.TwitchUserId, user2.TwitchUserId);

                Assert.That(updatedUser,                Is.Not.Null);
                Assert.That(updatedUser.LockedDateTime, Is.Null);
                Assert.That(updatedUser.LockedBy,       Is.Not.Null);
                Assert.That(updatedUser.LockedByUser,   Is.EqualTo(user1));
            }
        }

        [Test]
        public void Test_AddOrUpdateUser_AddWorks()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                const int    twitchUserId = 42;
                const string userName     = "userName";


                //Act
                var updatedUser = Repository.AddOrUpdateUser(context,
                                                             twitchUserId,
                                                             userName,
                                                             userName,
                                                             "",
                                                             "",
                                                             0
                                                            );

                Assert.That(updatedUser,              Is.Not.Null);
                Assert.That(updatedUser.TwitchUserId, Is.EqualTo(twitchUserId));
                Assert.That(updatedUser.Name,         Is.EqualTo(userName));

                Assert.That(context.Users.Single()
                                   .TwitchUserId,
                            Is.EqualTo(twitchUserId)
                           );
            }
        }

        [Test]
        public void Test_AddOrUpdateUser_UpdateWorks()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                const int    twitchUserId = 42;
                const string userName     = "userName";
                const string newEMail     = "mail";

                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);
                context.SaveChanges();

                //Act
                var updatedUser = Repository.AddOrUpdateUser(context,
                                                             twitchUserId,
                                                             userName,
                                                             userName,
                                                             newEMail,
                                                             "",
                                                             0
                                                            );

                Assert.That(updatedUser,              Is.Not.Null);
                Assert.That(updatedUser.TwitchUserId, Is.EqualTo(twitchUserId));
                Assert.That(updatedUser.Name,         Is.EqualTo(userName));
                Assert.That(updatedUser.Email,        Is.EqualTo(newEMail));
            }
        }
    }
}