using System;
using System.Linq;
using System.Threading.Tasks;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.TwitchClient;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Extensions;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Test.BusinessLogic.TwitchClient.Helpers;

namespace Test.BusinessLogic.TwitchClient
{
    [Explicit("Needs user Interaction (TwitchLogin in Browser)")]
    public class TestTwitchClient
    {
        public IOptions<TwitchSettings>        TwitchModel          { get; set; }
        public TwitchTokenRefreshResponseModel RefreshResponseModel { get; set; }
        public IServiceProvider                ServiceProvider      { get; set; }
        public ITwitchClient                   Client               { get; set; }

        [SetUp]
        public async Task Setup()
        {
            void SetupServiceCollection()
            {
                var configuration = new ConfigurationBuilder().AddEnvironmentVariables()
                                                              .Build();
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddTwitchClient();

                serviceCollection.Configure<TwitchSettings>(configuration.GetSection(TwitchSettings.cSectionName));
                serviceCollection.AddSingleton<IOptions<UserManagementSettings>>(new OptionsWrapper<UserManagementSettings>(new UserManagementSettings(32155062, Array.Empty<long>())));

                ServiceProvider = serviceCollection.BuildServiceProvider();
            }

            SetupServiceCollection();

            TwitchModel          = ServiceProvider.GetService<IOptions<TwitchSettings>>();
            RefreshResponseModel = await TokenModelHelper.GetTokenModelAsync(TwitchModel.Value);
            Client               = ServiceProvider.GetRequiredService<ITwitchClient>();
            RefreshResponseModel = await Client.RefreshTokenAsync(RefreshResponseModel.NewAccessToken, RefreshResponseModel.RefreshToken);

            TokenModelHelper.SaveTokenModel(RefreshResponseModel);
        }

        [Test]
        public async Task Test_AiO()
        {
            var user                       = await Client.GetUserAsync(RefreshResponseModel.NewAccessToken);
            var subscriptionResponseModels = await Client.GetTwitchSubscriptionModelsAsync(RefreshResponseModel.NewAccessToken, user.Id);

            Assert.That(subscriptionResponseModels,         Is.Not.Null);
            Assert.That(subscriptionResponseModels.Count(), Is.AtLeast(1));
        }
    }
}