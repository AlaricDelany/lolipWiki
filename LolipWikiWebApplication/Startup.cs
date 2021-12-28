using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Twitch;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.BusinessLogic.Providers;
using LolipWikiWebApplication.BusinessLogic.TwitchClient;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LolipWikiWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IProvideCurrentVersion, ProvideCurrentVersion>();

#region Settings

            RegisterSetting<TwitchSettings>(services, TwitchSettings.cSectionName);
            RegisterSetting<UserManagementSettings>(services, UserManagementSettings.cSectionName);

#endregion Settings


#region Clients

            services.AddTransient<ITwitchClient, TwitchClient>();

#endregion Clients

#region Repositories

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<IConfigurationRepository, ConfigurationRepository>();

#endregion Repositories

#region Logic

            services.AddTransient<IUserManagementLogic, UserManagementLogic>();
            services.AddTransient<IAccessControlLogic, AccessControlLogic>();
            services.AddTransient<IArticleLogic, ArticleLogic>();
            services.AddTransient<IConfigurationLogic, ConfigurationLogic>();

#endregion Logic

            AddTwitchLogin(services);
            
            services.AddRazorPages();
            services.AddHttpClient();
            services.AddControllers()
                    .AddJsonOptions(options =>
                                    {
                                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                                    }
                                   );
            RegisterDatabase(services);
        }

        private void RegisterDatabase(IServiceCollection services)
        {
            services.AddDbContext<ILolipWikiDbContext, LolipWikiDbContext>(options =>
                                                                           {
                                                                               options.UseLazyLoadingProxies();
                                                                               options.UseSqlServer(Configuration.GetConnectionString("LolipWikiDb"),
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

        private static void AddTwitchLogin(IServiceCollection services)
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
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                //app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            else
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapRazorPages();
                                 endpoints.MapControllers();
                             }
                            );
            InitializeDatabase(app);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<LolipWikiDbContext>())
                {
                    dbContext.EnsureIsUpToDate(IUser.cUserRoleDefinitions);
                }
            }
        }

        private TSettingsModel RegisterSetting<TSettingsModel>(IServiceCollection services, string sectionName) where TSettingsModel : class
        {
            IConfigurationSection configurationSection = Configuration.GetSection(sectionName);

            services.Configure<TSettingsModel>(configurationSection);

            return configurationSection.Get<TSettingsModel>();
        }
    }
}