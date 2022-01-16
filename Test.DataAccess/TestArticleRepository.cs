using System;
using System.Linq;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Test.DataAccess
{
    public class TestArticleRepository
    {
        public IServiceProvider   ServiceProvider { get; set; }
        public IArticleRepository Repository      { get; set; }

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

            Repository = ServiceProvider.GetService<IArticleRepository>();
        }

        [Test]
        [Ignore("No Publishing implemented right now")]
        public void Test_GetActiveVersions_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);

                var articleEM = new ArticleEM(userEM);
                context.Articles.Add(articleEM);

                var articleVersion01 = new ArticleVersionEM("01",
                                                            "",
                                                            "",
                                                            0,
                                                            DateTime.MinValue,
                                                            articleEM,
                                                            userEM
                                                           );
                var articleVersion02 = new ArticleVersionEM("02",
                                                            "",
                                                            "",
                                                            1,
                                                            DateTime.MinValue,
                                                            articleEM,
                                                            userEM
                                                           );

                articleVersion01.PublishedAt = DateTime.Now;

                articleEM.Versions.Add(articleVersion01);
                articleEM.Versions.Add(articleVersion02);

                context.SaveChanges();

                //Act
                var version = Repository.GetActiveVersions(context);

                Assert.That(version.Single(), Is.EqualTo(articleVersion01));
            }
        }

        [Test]
        [Ignore("No Publishing implemented right now")]
        public void Test_GetActiveVersions_ReturnsTheLatestVersion()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);

                var articleEM = new ArticleEM(userEM);
                context.Articles.Add(articleEM);

                var articleVersion01 = new ArticleVersionEM("01",
                                                            "",
                                                            "",
                                                            0,
                                                            DateTime.MinValue,
                                                            articleEM,
                                                            userEM
                                                           );
                var articleVersion02 = new ArticleVersionEM("02",
                                                            "",
                                                            "",
                                                            1,
                                                            DateTime.MinValue,
                                                            articleEM,
                                                            userEM
                                                           );

                articleVersion02.PublishedAt = DateTime.MaxValue;

                articleEM.Versions.Add(articleVersion01);
                articleEM.Versions.Add(articleVersion02);

                context.SaveChanges();

                //Act
                var version = Repository.GetActiveVersions(context);

                Assert.That(version.Single(), Is.EqualTo(articleVersion02));
            }
        }

        [Test]
        public void Test_Add_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
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
                var version = Repository.Add(context,
                                             userEM.TwitchUserId,
                                             "Title",
                                             ""
                                            );

                Assert.That(version,             Is.Not.Null);
                Assert.That(version.Article,     Is.Not.Null);
                Assert.That(version.Content,     Is.Not.Null.Or.Empty);
                Assert.That(version.CreatedBy,   Is.EqualTo(userEM));
                Assert.That(version.PublishedAt, Is.Null);
            }
        }

        [Test]
        public void Test_Update_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                const string newContent = "new Content";

                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);
                context.SaveChanges();

                var version = Repository.Add(context,
                                             userEM.TwitchUserId,
                                             "Title",
                                             ""
                                            );
                context.SaveChanges();


                //Act
                var newVersion = Repository.Update(context,
                                                   userEM.TwitchUserId,
                                                   version.Id,
                                                   newContent
                                                  );
                context.SaveChanges();

                Assert.That(newVersion,             Is.Not.Null);
                Assert.That(newVersion.Article,     Is.Not.Null);
                Assert.That(newVersion.Content,     Is.EqualTo(newContent));
                Assert.That(newVersion.CreatedBy,   Is.EqualTo(userEM));
                Assert.That(newVersion.PublishedAt, Is.Null);
                Assert.That(newVersion.ChangedAt,   Is.Not.Null);
                Assert.That(newVersion.ChangedById, Is.EqualTo(userEM.Id));
                Assert.That(newVersion.ChangedBy,   Is.EqualTo(userEM));
                Assert.That(newVersion.Revision,    Is.EqualTo(0));
            }
        }

        [Test]
        public void Test_AddVersion_Works()
        {
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var userEM = new UserEM(42,
                                        "Name",
                                        "Name",
                                        "",
                                        "",
                                        0
                                       );
                context.Users.Add(userEM);
                context.SaveChanges();

                var version = Repository.Add(context,
                                             userEM.TwitchUserId,
                                             "Title",
                                             ""
                                            );
                context.SaveChanges();


                //Act
                var newVersion = Repository.AddVersion(context, version, userEM.TwitchUserId);
                context.SaveChanges();

                Assert.That(newVersion,             Is.Not.Null);
                Assert.That(newVersion.Article,     Is.Not.Null);
                Assert.That(newVersion.Title,       Is.EqualTo(version.Title));
                Assert.That(newVersion.Content,     Is.EqualTo(version.Content));
                Assert.That(newVersion.CreatedBy,   Is.EqualTo(userEM));
                Assert.That(newVersion.PublishedAt, Is.Null);
                Assert.That(newVersion.ChangedAt,   Is.Null);
                Assert.That(newVersion.ChangedById, Is.Null);
                Assert.That(newVersion.ChangedBy,   Is.Null);
                Assert.That(newVersion.Revision,    Is.EqualTo(version.Revision + 1));
            }
        }
    }
}