namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Sp_UpdateSyncTransactionByField : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_UpdateSyncTransactionByField]";
        public override void Up()
        {
            String tsql = @"UPDATE [Sync].[Transactions]
                            SET [TransactionRefNo] = @TransactionRefNo,
                                [Cashier] = @Cashier,
                                [SyncStatus] = @SyncStatus,
                                [IsSyncReady] = @IsSyncReady,
                                [StockItemNo] = @StockItemNo,
                                [StockRefNo] = @StockRefNo,
                                [StockItemCode] = @StockItemCode,
	                            [StockDetails] = @StockDetails,
                                [StockCategoryRefNo] = @StockCategoryRefNo,
                                [StockCategoryLine] = @StockCategoryLine,
                                [StockUnitLeft] = @StockUnitLeft,
                                [StockUnitPurchased] = @StockUnitPurchased,
                                [StockUnitAmount] = @StockUnitAmount,
                                [StockCostAmount] = @StockCostAmount,
                                [StockDiscountAmount] = @StockDiscountAmount,
                                [RefModifiedDate] = @RefModifiedDate,
                                [RefCreatedDate] = @RefCreatedDate,
                                [CustomerName] = @CustomerName,
                                [CustomerEmail] = @CustomerEmail,
                                [CustomerAddress] = @CustomerAddress,
                                [CustomerTel] = @CustomerTel,
                                [CustomerGender] = @CustomerGender,
                                [CustomerDOB] = @CustomerDOB,
                                [Barcode] = @Barcode,
                                [TaxAmount] = @TaxAmount,
                                [IsDeleted] = @IsDeleted,
                                [StockReorderUnit] = @StockReorderUnit
                            WHERE [Id] = @Id";

            CreateStoredProcedure(dboName,
                t => new
                {
                    Id = t.Guid(Guid.NewGuid()),
                    TransactionRefNo = t.String(50),
                    Cashier = t.String(),
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
                    IsDeleted = t.Boolean(false),
                    StockReorderUnit = t.Int(),
                }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}
