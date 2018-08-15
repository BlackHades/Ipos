namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sp_UpdateSyncSpoilByField : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_UpdateSyncSpoilByField]";
        public override void Up()
        {
            String tsql = @"UPDATE [Sync].[Spoils] SET
                              [StockRefNo] = @StockRefNo,
                              [StockDetails] = @StockDetails,
                              [StockUnit] = @StockUnit,
                              [StockUnitLeft] = @StockUnitLeft,
                              [IsSyncReady] = @IsSyncReady,
                              [SyncStatus] = @IsSyncReady,
                              [SpoilDetails] = @SpoilDetails,
                              [ReportedBy] = @ReportedBy,
                              [MachineName] = @MachineName,
                              [RefCreatedDate] = @RefCreatedDate,
                              [RefModifiedDate] = @RefModifiedDate,
                              [Cost] = @Cost,
                              [IsDeleted] = 0
                             WHERE Id = @Id";

            CreateStoredProcedure(dboName,
                t => new
                {
                    StockRefNo = t.Int(),
                    StockDetails = t.String(250, unicode: false),
                    StockUnit = t.Int(),
                    StockUnitLeft = t.Int(null, "null"),
                    IsSyncReady = t.Boolean(defaultValue: false),
                    SyncStatus = t.Int(),
                    SpoilDetails = t.String(250, unicode: false),
                    ReportedBy = t.String(),
                    MachineName = t.String(150, unicode: false),
                    RefCreatedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    RefModifiedDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    Cost = t.Double(null, "null"),
                    Id = t.Guid(Guid.NewGuid())
                }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}
