using System;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Test.DataAccess
{
    public class TestConfigurationRepository
    {
        public IServiceProvider         ServiceProvider { get; set; }
        public IConfigurationRepository Repository      { get; set; }

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

            Repository = ServiceProvider.GetService<IConfigurationRepository>();
        }

        [Test]
        public void Test_GetThrowsWhenTableIsEmpty()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var config = Repository.Get(context);
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
                context.Configurations.AddRange(new[]
                                                {
                                                    new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone),
                                                    new ConfigurationEM(AccessControlType.RoleOnly, AccessControlType.RoleOnly)
                                                }
                                               );

                context.SaveChanges();

                Assert.Throws<InvalidOperationException>(() =>
                                                         {
                                                             var config = Repository.Get(context);
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
                context.Configurations.Add(new ConfigurationEM(AccessControlType.Everyone, AccessControlType.Everyone));

                context.SaveChanges();

                var config = Repository.Get(context);

                Assert.That(config,                             Is.Not.Null);
                Assert.That(config.ReadArticleControlTypeEnum,  Is.EqualTo(AccessControlType.Everyone));
                Assert.That(config.WriteArticleControlTypeEnum, Is.EqualTo(AccessControlType.Everyone));
            }

            Assert.Pass();
        }
    }
}