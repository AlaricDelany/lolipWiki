using System.Text.Json.Serialization;
using LolipWikiWebApplication.BusinessLogic.Extensions;
using LolipWikiWebApplication.BusinessLogic.Model.Settings;
using LolipWikiWebApplication.BusinessLogic.Model.UserManagement;
using LolipWikiWebApplication.BusinessLogic.Providers;
using LolipWikiWebApplication.BusinessLogic.TwitchClient.Extensions;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.Configure<TwitchSettings>(Configuration.GetSection(TwitchSettings.cSectionName));
            services.Configure<UserManagementSettings>(Configuration.GetSection(UserManagementSettings.cSectionName));

#endregion Settings

            services.AddTwitchClient();
            services.AddRepositories();
            services.AddLogics();
            services.AddTwitchLogin();
            services.AddLolipWikiDb(Configuration);

            services.AddRazorPages();
            services.AddControllers()
                    .AddJsonOptions(options =>
                                    {
                                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                                    }
                                   );
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
    }
}