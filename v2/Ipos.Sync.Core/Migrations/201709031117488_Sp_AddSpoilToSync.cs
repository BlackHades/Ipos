namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sp_AddSpoilToSync : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_AddSpoilToSync]";
        public override void Up()
        {
            String tsql = @"INSERT INTO [Sync].[Spoils]
                           ([Id]
                           ,[TransactionRefNo]
                           ,[StockRefNo]
                           ,[StockDetails]
                           ,[StockUnit]
                           ,[StockUnitLeft]
                           ,[IsSyncReady]
                           ,[SyncStatus]
                           ,[SpoilDetails]
                           ,[ReasonSyncFailed]
                           ,[ReportedBy]
                           ,[SyncRefCreatedOn]
                           ,[SyncRefModifiedOn]
                           ,[MachineName]
                           ,[SyncRefNo]
                           ,[SyncDate]
                           ,[SyncFailedCount]
                           ,[RefCreatedDate]
                           ,[RefModifiedDate]
                           ,[Cost]
                           ,[CreatedOnUtc]
                           ,[ModifiedOnUtc]
                           ,[IsDeleted])
                     VALUES
                           (@Id,
                           @TransactionRefNo,
                           @StockRefNo,
                           @StockDetails,
                           @StockUnit,
                           @StockUnitLeft,
                           @IsSyncReady,
                           @SyncStatus,
                           @SpoilDetails,
                           @ReasonSyncFailed,
                           @ReportedBy,
                           @SyncRefCreatedOn,
                           @SyncRefModifiedOn,
                           @MachineName,
                           @SyncRefNo,
                           @SyncDate,
                           @SyncFailedCount,
                           @RefCreatedDate,
                           @RefModifiedDate,
                           @Cost,
                          @CreatedOnUtc,
                           @ModifiedOnUtc,
                           @IsDeleted)";

            CreateStoredProcedure(dboName,
                t => new
                {
                    Id = t.Guid(Guid.NewGuid()),
                    TransactionRefNo = t.String(50),
                    StockRefNo = t.Int(),
                    StockDetails = t.String(250, unicode: false),
                    StockUnit = t.Int(),
                    StockUnitLeft = t.Int(null, "null"),
                    IsSyncReady = t.Boolean(defaultValue: false),
                    SyncStatus = t.Int(),
                    SpoilDetails = t.String(250, unicode: false),
                    ReasonSyncFailed = t.String(defaultValue: null, defaultValueSql: "null"),
                    ReportedBy = t.String(),
                    MachineName = t.String(150, unicode: false),
                    SyncRefCreatedOn = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncRefModifiedOn = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncRefNo = t.String(defaultValue: null, defaultValueSql: "null"),
                    SyncDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncFailedCount = t.Int(),
                    RefCreatedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    RefModifiedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    Cost = t.Double(null, "null"),
                    CreatedOnUtc = t.DateTime(),
                    ModifiedOnUtc = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    IsDeleted = t.Boolean(false)
                }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}
