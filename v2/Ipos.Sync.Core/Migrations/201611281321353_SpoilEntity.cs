namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpoilEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Sync.Spoils",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    TransactionRefNo = c.String(maxLength: 50, unicode: false),
                    StockRefNo = c.Int(nullable: false),
                    StockDetails = c.String(maxLength: 250, unicode: false),
                    StockUnit = c.Int(nullable: false),
                    StockUnitLeft = c.Int(),
                    IsSyncReady = c.Boolean(nullable: false),
                    SyncStatus = c.Int(nullable: false),
                    SpoilDetails = c.String(maxLength: 250, unicode: false),
                    ReasonSyncFailed = c.String(),
                    ReportedBy = c.String(),
                    SyncRefCreatedOn = c.DateTime(),
                    SyncRefModifiedOn = c.DateTime(),
                    MachineName = c.String(maxLength: 150, unicode: false),
                    SyncRefNo = c.String(),
                    SyncDate = c.DateTime(),
                    SyncFailedCount = c.Int(nullable: false),
                    RefCreatedDate = c.DateTime(),
                    RefModifiedDate = c.DateTime(),
                    Cost = c.Double(),
                    CreatedOnUtc = c.DateTime(nullable: false),
                    ModifiedOnUtc = c.DateTime(),
                    IsDeleted = c.Boolean(nullable: false),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("Sync.Spoils");
        }
    }
}