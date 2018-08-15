namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sp_UpdateSyncSpoil : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_UpdateSyncSpoil]";
        public override void Up()
        {
            String tsql = @"UPDATE [Sync].[Spoils]
                               SET IsSyncReady = @IsSyncReady,
	                            SyncDate = @SyncDate,
	                            SyncRefNo = @SyncRefNo,
	                            SyncRefCreatedOn = @SyncRefCreatedOn,
	                            SyncRefModifiedOn = @SyncRefModifiedOn,
	                            ReasonSyncFailed = @ReasonSyncFailed,
                                SyncFailedCount= @SyncFailedCount
                             WHERE Id = @Id";

            CreateStoredProcedure(dboName,
                t => new
                {
                    IsSyncReady = t.Boolean(defaultValue: false),
                    SyncDate = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncRefNo = t.String(150, unicode: false),
                    SyncRefCreatedOn = t.DateTime(defaultValue: null, defaultValueSql: "null"),
                    SyncRefModifiedOn = t.DateTime(defaultValue:null, defaultValueSql: "null"),
                    ReasonSyncFailed = t.String(150, unicode: false),
                    SyncFailedCount = t.Int(),
                    Id = t.Guid(Guid.NewGuid())
                }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}
