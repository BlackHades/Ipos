namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelTransactionChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("Sync.Transactions", "SyncFailedCount", c => c.Int(nullable: false));
            AddColumn("Sync.Transactions", "SyncRefNo", c => c.String(maxLength: 150, unicode: false));
            AddColumn("Sync.Transactions", "SyncRefCreatedOn", c => c.DateTime());
            AddColumn("Sync.Transactions", "SyncRefModifiedOn", c => c.DateTime());
            AddColumn("Sync.Transactions", "StockReorderUnit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Sync.Transactions", "StockReorderUnit");
            DropColumn("Sync.Transactions", "SyncRefModifiedOn");
            DropColumn("Sync.Transactions", "SyncRefCreatedOn");
            DropColumn("Sync.Transactions", "SyncRefNo");
            DropColumn("Sync.Transactions", "SyncFailedCount");
        }
    }
}
