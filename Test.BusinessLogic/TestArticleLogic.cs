using System;
using System.Linq;
using LolipWikiWebApplication.BusinessLogic.Extensions;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.EntityModels;
using LolipWikiWebApplication.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Test.BusinessLogic
{
    public class TestArticleLogic
    {
        public IArticleLogic    Logic           { get; set; }
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
                serviceCollection.AddLogics();

                ServiceProvider = serviceCollection.BuildServiceProvider();
            }

            SetupServiceCollection();

            Logic = ServiceProvider.GetService<IArticleLogic>();
        }

        [Test]
        public void Test_GetActiveVersions_Filter_Works()
        {
            var userEM = new UserEM(0,
                                    "",
                                    "",
                                    "",
                                    "",
                                    0
                                   );
            using (var context = ServiceProvider.GetRequiredService<ILolipWikiDbContext>())
            {
                var article01 = new ArticleEM(userEM);
                var article02 = new ArticleEM(userEM);
                var article03 = new ArticleEM(userEM);

                context.Users.Add(userEM);
                context.Articles.Add(article01);
                context.Articles.Add(article02);
                context.Articles.Add(article03);
                var version = context.ArticleVersions.Add(new ArticleVersionEM("Test01",
                                                                               "",
                                                                               "",
                                                                               0,
                                                                               DateTime.MinValue,
                                                                               article01,
                                                                               userEM
                                                                              )
                                                         )
                                     .Entity;
                version.PublishedAt = DateTime.MinValue;

                context.ArticleVersions.Add(new ArticleVersionEM("Test02",
                                                                 "",
                                                                 "",
                                                                 0,
                                                                 DateTime.MinValue,
                                                                 article02,
                                                                 userEM
                                                                )
                                           );
                context.ArticleVersions.Add(new ArticleVersionEM("Test24",
                                                                 "",
                                                                 "",
                                                                 0,
                                                                 DateTime.MinValue,
                                                                 article03,
                                                                 userEM
                                                                )
                                           );
                version = context.ArticleVersions.Add(new ArticleVersionEM("Test42",
                                                                           "",
                                                                           "",
                                                                           0,
                                                                           DateTime.MinValue,
                                                                           article02,
                                                                           userEM
                                                                          )
                                                     )
                                 .Entity;
                version.PublishedAt = DateTime.MinValue;

                context.SaveChanges();

                var versions = Logic.GetActiveVersions(context,
                                                       null,
                                                       0,
                                                       10,
                                                       "42"
                                                      )
                                    .ToArray();

                Assert.That(versions,          Is.Not.Null);
                Assert.That(versions,          Is.Not.Empty);
                Assert.That(versions.Length,   Is.EqualTo(1));
                Assert.That(versions.Single().Id, Is.EqualTo(version.Id));
            }
        }
    }
}