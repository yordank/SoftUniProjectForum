using Microsoft.Owin;
using Owin;
using Forum.Migrations;
using Forum.Models;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(Forum.Startup))]
namespace Forum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ForumDbContext, Configuration>());

            ConfigureAuth(app);
        }
    }
}
