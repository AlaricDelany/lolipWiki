using System;
using LolipWikiWebApplication.BusinessLogic.BusinessModels;
using LolipWikiWebApplication.BusinessLogic.Exceptions;
using LolipWikiWebApplication.BusinessLogic.Extensions;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using LolipWikiWebApplication.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Test.BusinessLogic
{
    public class TestAccessControlLogic
    {
        public IAccessControlLogic Logic           { get; set; }
        public IServiceProvider    ServiceProvider { get; set; }

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
                serviceCollection.AddLogics();

                ServiceProvider = serviceCollection.BuildServiceProvider();
            }

            SetupServiceCollection();

            Logic = ServiceProvider.GetService<IAccessControlLogic>();
        }

#region EnsureReadIsAllowed

        [Test]
        public void Test_EnsureReadIsAllowed_LockedUser_Throws_UnauthorizedAccessException()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.LockedSince)
                    .Returns(() => DateTime.MinValue);

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));
                context.SaveChanges();

                var exception = Assert.Throws<UserIsLockedException>(() =>
                                                                     {
                                                                         Logic.EnsureReadIsAllowed(context, userMock.Object);
                                                                     }
                                                                    );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserIsLockedException.GetMessage(userMock.Object.Id)));
            }
        }

        [Test]
        public void Test_EnsureReadIsAllowed_AccessControlType_Everyone_Works()
        {
            var userMock = new Mock<IUser>();

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.RoleOnly));
                context.SaveChanges();

                Logic.EnsureReadIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureReadIsAllowed_AccessControlType_SubOnly_Works()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.SubscriptionState)
                    .Returns(() => SubscriptionStateType.T1);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.SubOnly, AccessControlType.RoleOnly));
                context.SaveChanges();

                Logic.EnsureReadIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureReadIsAllowed_AccessControlType_SubOnly_ThrowsWhenNotSub()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.SubscriptionState)
                    .Returns(() => SubscriptionStateType.None);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.SubOnly, AccessControlType.RoleOnly));
                context.SaveChanges();

                var exception = Assert.Throws<UserSubMissingException>(() =>
                                                                       {
                                                                           Logic.EnsureReadIsAllowed(context, userMock.Object);
                                                                       }
                                                                      );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserSubMissingException.GetMessage(userMock.Object.Id)));
            }
        }

        [Test]
        public void Test_EnsureReadIsAllowed_AccessControlType_RoleOnly_Works()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.IsArticleManager)
                    .Returns(() => true);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.RoleOnly));
                context.SaveChanges();

                Logic.EnsureReadIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureReadIsAllowed_AccessControlType_RoleOnly_ThrowsWhenNoRole()
        {
            var userMock = new Mock<IUser>();


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.RoleOnly));
                context.SaveChanges();

                var exception = Assert.Throws<UserRoleMissingException>(() =>
                                                                        {
                                                                            Logic.EnsureReadIsAllowed(context, userMock.Object);
                                                                        }
                                                                       );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserRoleMissingException.GetMessage(userMock.Object.Id)));
            }
        }

#endregion EnsureReadIsAllowed

#region EnsureWriteIsAllowed

        [Test]
        public void Test_EnsureWriteIsAllowed_LockedUser_Throws_UnauthorizedAccessException()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.LockedSince)
                    .Returns(() => DateTime.MinValue);

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));
                context.SaveChanges();

                var exception = Assert.Throws<UserIsLockedException>(() =>
                                                                     {
                                                                         Logic.EnsureWriteIsAllowed(context, userMock.Object);
                                                                     }
                                                                    );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserIsLockedException.GetMessage(userMock.Object.Id)));
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AccessControlType_Everyone_Works()
        {
            var userMock = new Mock<IUser>();

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));
                context.SaveChanges();

                Logic.EnsureWriteIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AccessControlType_SubOnly_Works()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.SubscriptionState)
                    .Returns(() => SubscriptionStateType.T1);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.SubOnly));
                context.SaveChanges();

                Logic.EnsureWriteIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AccessControlType_SubOnly_ThrowsWhenNotSub()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.SubscriptionState)
                    .Returns(() => SubscriptionStateType.None);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.SubOnly));
                context.SaveChanges();

                var exception = Assert.Throws<UserSubMissingException>(() =>
                                                                       {
                                                                           Logic.EnsureWriteIsAllowed(context, userMock.Object);
                                                                       }
                                                                      );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserSubMissingException.GetMessage(userMock.Object.Id)));
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AccessControlType_RoleOnly_Works()
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(x => x.IsArticleManager)
                    .Returns(() => true);


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.RoleOnly));
                context.SaveChanges();

                Logic.EnsureWriteIsAllowed(context, userMock.Object);
                Assert.Pass();
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AccessControlType_RoleOnly_ThrowsWhenNoRole()
        {
            var userMock = new Mock<IUser>();


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.RoleOnly));
                context.SaveChanges();

                var exception = Assert.Throws<UserRoleMissingException>(() =>
                                                                        {
                                                                            Logic.EnsureWriteIsAllowed(context, userMock.Object);
                                                                        }
                                                                       );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserRoleMissingException.GetMessage(userMock.Object.Id)));
            }
        }

        [Test]
        public void Test_EnsureWriteIsAllowed_AlsoChecksRead()
        {
            var userMock = new Mock<IUser>();


            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.Everyone));
                context.SaveChanges();

                var exception = Assert.Throws<UserRoleMissingException>(() =>
                                                                        {
                                                                            Logic.EnsureWriteIsAllowed(context, userMock.Object);
                                                                        }
                                                                       );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserRoleMissingException.GetMessage(userMock.Object.Id)));
            }
        }

#endregion EnsureWriteIsAllowed

#region EnsureIsAllowed

        [Test]
        public void Test_EnsureIsAllowed_LockedUser_Throws_UnauthorizedAccessException()
        {
            var userEM = new UserEM(0,
                                    "",
                                    "",
                                    "",
                                    "",
                                    0
                                   )
            {
                LockedDateTime = DateTime.MinValue
            };

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));

                context.Users.Add(userEM);
                context.SaveChanges();

                var exception = Assert.Throws<UserIsLockedException>(() =>
                                                                     {
                                                                         Logic.EnsureIsAllowed(context, new UserBM(userEM), IUser.cRoleNameAdmin);
                                                                     }
                                                                    );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(UserIsLockedException.GetMessage(userEM.TwitchUserId)));
            }
        }

        [Test]
        public void Test_EnsureIsAllowed_NoRoles_Throws()
        {
            var roleShortName = IUser.cRoleNameUserManager;
            var userEM = new UserEM(0,
                                    "",
                                    "",
                                    "",
                                    "",
                                    0
                                   );

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));

                context.Users.Add(userEM);
                context.SaveChanges();

                var exception = Assert.Throws<RoleNotFoundException>(() =>
                                                                     {
                                                                         Logic.EnsureIsAllowed(context, new UserBM(userEM), roleShortName);
                                                                     }
                                                                    );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo(RoleNotFoundException.GetMessage(roleShortName, userEM.TwitchUserId)));
            }
        }

        [Test]
        public void Test_EnsureIsAllowed_Works()
        {
            var roleShortName = IUser.cRoleNameUserManager;
            var userEM = new UserEM(0,
                                    "",
                                    "",
                                    "",
                                    "",
                                    0
                                   );
            var roleEM = new RoleEM(roleShortName, roleShortName);

            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));
                context.Roles.Add(roleEM);
                context.Users.Add(userEM);
                context.UserRoles.Add(new UserRoleEM(userEM, roleEM));
                context.SaveChanges();

                Logic.EnsureIsAllowed(context, new UserBM(userEM), roleShortName);
                Assert.Pass();
            }
        }

#endregion EnsureIsAllowed
    }
}