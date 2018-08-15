namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Sync.Spoils", "TransactionRefNo", c => c.String(maxLength: 50, unicode: false));
            AlterColumn("Sync.Spoils", "MachineName", c => c.String(maxLength: 150, unicode: false));
            AlterColumn("Sync.Spoils", "SyncRefNo", c => c.String());
            AlterColumn("Sync.Spoils", "SyncDate", c => c.DateTime());
            AlterColumn("Sync.Spoils", "SyncFailedCount", c => c.Int(nullable: false));
            AlterColumn("Sync.Spoils", "RefCreatedDate", c => c.DateTime());
            AlterColumn("Sync.Spoils", "RefModifiedDate", c => c.DateTime());
            AlterColumn("Sync.Spoils", "Cost", c => c.Double());
            AlterColumn("Sync.Spoils", "StockRefNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Sync.Spoils", "StockRefNo", c => c.String(maxLength: 50, unicode: false));
            DropColumn("Sync.Spoils", "Cost");
            DropColumn("Sync.Spoils", "RefModifiedDate");
            DropColumn("Sync.Spoils", "RefCreatedDate");
            DropColumn("Sync.Spoils", "SyncFailedCount");
            DropColumn("Sync.Spoils", "SyncDate");
            DropColumn("Sync.Spoils", "SyncRefNo");
            DropColumn("Sync.Spoils", "MachineName");
            DropColumn("Sync.Spoils", "TransactionRefNo");
        }
    }
}
