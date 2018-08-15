using System.Data.Entity;
using SmartWr.Ipos.Core.Models.Mapping;
using SmartWr.WebFramework.Library.Infrastructure.Logging;
using SmartWr.WebFramework.Library.MiddleServices.DataAccess;

namespace SmartWr.Ipos.Core.Context
{
    public class IPosDbContext : ApplicationDbContext
    {
        public IPosDbContext()
            : base("name=IPosDbContext", NLogLogger.Instance)
        {
        }

        public IPosDbContext(string connectionName, ILogger logger)
            : base(connectionName, logger)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new aspnet_ApplicationsMap());
            modelBuilder.Configurations.Add(new aspnet_MembershipMap());
            modelBuilder.Configurations.Add(new aspnet_PathsMap());
            modelBuilder.Configurations.Add(new aspnet_PersonalizationAllUsersMap());
            modelBuilder.Configurations.Add(new aspnet_PersonalizationPerUserMap());
            modelBuilder.Configurations.Add(new aspnet_ProfileMap());
            modelBuilder.Configurations.Add(new aspnet_RolesMap());
            modelBuilder.Configurations.Add(new aspnet_SchemaVersionsMap());
            modelBuilder.Configurations.Add(new aspnet_UsersMap());
            modelBuilder.Configurations.Add(new aspnet_WebEvent_EventsMap());

            modelBuilder.Configurations.Add(new AuditMap());
            modelBuilder.Configurations.Add(new BulkSMMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new OrderDetailMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new SpoilMap());

            //modelBuilder.Configurations.Add(new StaffWeeklySaleMap());
            //modelBuilder.Configurations.Add(new HourlySalesOrderMap());
            //modelBuilder.Configurations.Add(new MonthlySalesOrderMap());
            //modelBuilder.Configurations.Add(new WeeklySalesOrderMap());
            //modelBuilder.Configurations.Add(new YearlySalesOrderMap());
        }

    }
}