using System.Data.Entity.Migrations;
using SmartWr.Ipos.Core.Context;
using SmartWr.Ipos.Core.Migrations.DataHelper;
using SmartWr.Ipos.Core.Settings;

namespace SmartWr.Ipos.Core.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<IPosDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IPosDbContext context)
        {
            if (IposConfig.UseMembership)
                UserDataHelper.ConfigureIposMemberShip(context);

            UserDataHelper.CreateRoles(context);
            var adminUser = UserDataHelper.CreateAdminUser(context);
            //UserDataHelper.CreateTestData(context, adminUser);
        }
    }
}