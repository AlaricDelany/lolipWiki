using LolipWikiWebApplication.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EntityFrameworkMigrationGeneratorConsoleApp
{
    /*
        Add-Migration CreateLolipWikiDb -Project LolipWikiWebApplication.DataAccess -StartupProject EntityFrameworkMigrationGeneratorConsoleApp
        







     */

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        // EF Core uses this method at design time to access the DbContext
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LolipWikiDbContext>(options =>
                                                      {
                                                          options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=LolipWikiDb;Trusted_Connection=True;",
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}