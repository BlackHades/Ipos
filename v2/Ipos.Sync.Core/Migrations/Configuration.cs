namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ipos.Sync.Core.Context.IposSyncContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Ipos.Sync.Core.Context.IposSyncContext context)
        {
        }
    }
}
