namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Sync.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReasonSyncFailed = c.String(),
                        TransactionRefNo = c.String(maxLength: 50, unicode: false),
                        CurrencyCode = c.String(maxLength: 50, unicode: false),
                        Cashier = c.String(maxLength: 150, unicode: false),
                        MachineName = c.String(maxLength: 50, unicode: false),
                        SyncDate = c.DateTime(),
                        SyncStatus = c.Int(nullable: false),
                        IsSyncReady = c.Boolean(nullable: false),
                        StockItemNo = c.String(),
                        StockRefNo = c.String(maxLength: 50, unicode: false),
                        StockItemCode = c.String(maxLength: 50, unicode: false),
                        StockDetails = c.String(maxLength: 250, unicode: false),
                        StockCategoryRefNo = c.String(),
                        StockCategoryLine = c.String(maxLength: 150, unicode: false),
                        StockUnitLeft = c.Int(),
                        StockUnitPurchased = c.Int(nullable: false),
                        StockUnitAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockCostAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockDiscountAmount = c.Double(nullable: false),
                        RefModifiedDate = c.DateTime(),
                        RefCreatedDate = c.DateTime(),
                        CustomerName = c.String(maxLength: 150, unicode: false),
                        CustomerEmail = c.String(maxLength: 50, unicode: false),
                        CustomerAddress = c.String(maxLength: 150, unicode: false),
                        CustomerTel = c.String(maxLength: 50, unicode: false),
                        CustomerGender = c.Int(),
                        CustomerDOB = c.DateTime(),
                        Barcode = c.String(maxLength: 100, unicode: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        ModifiedOnUtc = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Sync.Transactions");
        }
    }
}
