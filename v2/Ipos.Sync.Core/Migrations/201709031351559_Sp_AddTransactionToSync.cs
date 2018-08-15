namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Sp_AddTransactionToSync : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_AddTransactionToSync]";
        public override void Up()
        {
            String tsql = @"INSERT INTO [Sync].[Transactions]
                           ([Id]
                           ,[ReasonSyncFailed]
                           ,[TransactionRefNo]
                           ,[CurrencyCode]
                           ,[Cashier]
                           ,[MachineName]
                           ,[SyncDate]
                           ,[SyncStatus]
                           ,[IsSyncReady]
                           ,[StockItemNo]
                           ,[StockRefNo]
                           ,[StockItemCode]
                           ,[StockDetails]
                           ,[StockCategoryRefNo]
                           ,[StockCategoryLine]
                           ,[StockUnitLeft]
                           ,[StockUnitPurchased]
                           ,[StockUnitAmount]
                           ,[StockCostAmount]
                           ,[StockDiscountAmount]
                           ,[RefModifiedDate]
                           ,[RefCreatedDate]
                           ,[CustomerName]
                           ,[CustomerEmail]
                           ,[CustomerAddress]
                           ,[CustomerTel]
                           ,[CustomerGender]
                           ,[CustomerDOB]
                           ,[Barcode]
                           ,[TaxAmount]
                           ,[CreatedOnUtc]
                           ,[ModifiedOnUtc]
                           ,[IsDeleted]
                           ,[SyncFailedCount]
                           ,[SyncRefNo]
                           ,[SyncRefCreatedOn]
                           ,[SyncRefModifiedOn]
                           ,[StockReorderUnit])
                     VALUES
                           (@Id,
                           @ReasonSyncFailed,
                           @TransactionRefNo,
                           @CurrencyCode,
                           @Cashier,
                           @MachineName,
                           @SyncDate,
                           @SyncStatus,
                           @IsSyncReady,
                           @StockItemNo,
                           @StockRefNo,
                           @StockItemCode,
                           @StockDetails,
                           @StockCategoryRefNo,
                           @StockCategoryLine,
                           @StockUnitLeft,
                           @StockUnitPurchased,
                           @StockUnitAmount,
                           @StockCostAmount,
                           @StockDiscountAmount,
                           @RefModifiedDate,
                           @RefCreatedDate,
                           @CustomerName,
                           @CustomerEmail,
                           @CustomerAddress,
                           @CustomerTel,
                           @CustomerGender,
                           @CustomerDOB,
                           @Barcode,
                           @TaxAmount,
                           @CreatedOnUtc,
                           @ModifiedOnUtc,
                           @IsDeleted,
                           @SyncFailedCount,
                           @SyncRefNo,
                           @SyncRefCreatedOn,
                           @SyncRefModifiedOn,
                           @StockReorderUnit)";

            CreateStoredProcedure(dboName,
                t => new
                {
                    Id = t.Guid(Guid.NewGuid()),
                    ReasonSyncFailed = t.String(defaultValue: null, defaultValueSql: "null"),
                    TransactionRefNo = t.String(50),
                    CurrencyCode = t.String(50),
                    Cashier = t.String(),
                    MachineName = t.String(150, unicode: false),
                    SyncDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncStatus = t.Int(),
                    IsSyncReady = t.Boolean(defaultValue: false),
                    StockItemNo = t.String(),
                    StockRefNo = t.String(50, defaultValue: null, defaultValueSql: "null"),
                    StockItemCode = t.String(50, defaultValue: null, defaultValueSql: "null"),
                    StockDetails = t.String(250, defaultValue: null, defaultValueSql: "null", unicode: false),
                    StockCategoryRefNo = t.String(defaultValue: null, defaultValueSql: "null"),
                    StockCategoryLine = t.String(150, defaultValue: null, defaultValueSql: "null"),
                    StockUnitLeft = t.Int(null, "null"),
                    StockUnitPurchased = t.Int(),
                    StockUnitAmount = t.Decimal(18, 2),
                    StockCostAmount = t.Decimal(18, 2),
                    StockDiscountAmount = t.Double(),
                    RefModifiedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    RefCreatedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    CustomerName = t.String(150, defaultValueSql: "null", unicode: false),
                    CustomerEmail = t.String(50, defaultValue: null, defaultValueSql: "null"),
                    CustomerAddress = t.String(150, defaultValue: null, defaultValueSql: "null"),
                    CustomerTel = t.String(50, defaultValue: null, defaultValueSql: "null", unicode: false),
                    CustomerGender = t.Int(null, "null"),
                    CustomerDOB = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    Barcode = t.String(100, defaultValue: null, defaultValueSql: "null", unicode: false),
                    TaxAmount = t.Decimal(),
                    CreatedOnUtc = t.DateTime(),
                    ModifiedOnUtc = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    IsDeleted = t.Boolean(false),
                    SyncFailedCount = t.Int(),
                    SyncRefNo = t.String(defaultValue: null, defaultValueSql: "null"),
                    SyncRefCreatedOn = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncRefModifiedOn = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    StockReorderUnit = t.Int(),
                }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}
