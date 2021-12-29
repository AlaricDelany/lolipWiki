using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Twitch;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace LolipWikiWebApplication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwitchLogin(this IServiceCollection services)
        {
            using (var sp = services.BuildServiceProvider())
            {
                var twitchSettings = sp.GetService<IOptions<TwitchSettings>>();

                services.AddAuthentication(cfg =>
                                           {
                                               cfg.DefaultChallengeScheme = TwitchAuthenticationDefaults.AuthenticationScheme;
                                               cfg.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
                                           }
                                          )
                        .AddCookie(options =>
                                   {
                                       options.Events = new CookieAuthenticationEvents
                                       {
                                           OnValidatePrincipal = context =>
                                           {
                                               var tokens = context.Properties.GetTokens()
                                                                   .ToArray();

                                               var accessToken    = tokens.Single(x => x.Name == "access_token");
                                               var refreshToken   = tokens.Single(x => x.Name == "refresh_token");
                                               var expiresAtToken = tokens.Single(x => x.Name == "expires_at");
                                               var expiresAt      = DateTimeOffset.Parse(expiresAtToken.Value);

                                               if (expiresAt < DateTimeOffset.UtcNow)
                                               {
                                                   context.ShouldRenew = true;
                                                   context.RejectPrincipal();
                                               }

                                               return Task.CompletedTask;
                                           }
                                       };
                                   }
                                  )
                        .AddTwitch(cfg =>
                                   {
                                       cfg.ClientId           = twitchSettings.Value.ClientId;
                                       cfg.ClientSecret       = twitchSettings.Value.ClientSecret;
                                       cfg.ReturnUrlParameter = twitchSettings.Value.RedirectUrl;
                                       cfg.ForceVerify        = true;
                                       cfg.SaveTokens         = true;
                                       cfg.CallbackPath       = PathString.FromUriComponent(twitchSettings.Value.RedirectUrlPath);

                                       cfg.Events = new OAuthEvents();

                                       var scopes = new List<string>
                                       {
                                           "user:read:email",
                                           "user:read:subscriptions"
                                       };
                                       cfg.Scope.Add(string.Join(" ", scopes));

                                       cfg.Validate();
                                   }
                                  );

                return services;
            }
        }

        public static void AddLolipWikiDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ILolipWikiDbContext, LolipWikiDbContext>(options =>
                                                                           {
                                                                               options.UseLazyLoadingProxies();
                                                                               options.UseSqlServer(configuration.GetConnectionString("LolipWikiDb"),
                                                                                                    x =>
                                                                                                    {
                                                                                                        x.MigrationsAssembly(typeof(LolipWikiDbContext).Assembly.GetName()
                                                                                                                                                       .Name
                                                                                                                            );
                                                                                                    }
                                                                                                   );
                                                                           }
                                                                          );
        }
    }
}