using System;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Test.LolipWikiWebApplication.DataAccess
{
    public class TestConfigurationRepository
    {
        public IServiceProvider ServiceProvider { get; set; }

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
        }

        [Test]
        public void Test_GetThrowsWhenTableIsEmpty()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var repo = ServiceProvider.GetService<IConfigurationRepository>();

                Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var config = repo.Get(context);
                                                         }
                                                        );
            }

            Assert.Pass();
        }

        [Test]
        public void Test_GetThrowsWhenTableHasMoreThanOneEntry()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var repo = ServiceProvider.GetService<IConfigurationRepository>();

                context.Configurations.AddRange(new[]
                                                {
                                                    new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone),
                                                    new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.RoleOnly)
                                                }
                                               );

                context.SaveChanges();

                Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var config = repo.Get(context);
                                                         }
                                                        );
            }

            Assert.Pass();
        }

        [Test]
        public void Test_GetWorks()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var repo = ServiceProvider.GetService<IConfigurationRepository>();

                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));

                context.SaveChanges();

                var config = repo.Get(context);

                Assert.That(config,                             Is.Not.Null);
                Assert.That(config.ReadArticleControlTypeEnum,  Is.EqualTo(AccessControlType.Everyone));
                Assert.That(config.WriteArticleControlTypeEnum, Is.EqualTo(AccessControlType.Everyone));
            }

            Assert.Pass();
        }
    }
}