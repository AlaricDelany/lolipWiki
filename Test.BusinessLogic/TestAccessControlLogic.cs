using System;
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

                var exception = Assert.Throws<UnauthorizedAccessException>(() =>
                                                                           {
                                                                               Logic.EnsureReadIsAllowed(context, userMock.Object);
                                                                           }
                                                                          );

                Assert.That(exception,         Is.Not.Null);
                Assert.That(exception.Message, Is.EqualTo($"User:{userMock.Object.Id} is locked."));
            }
        }
    }
}