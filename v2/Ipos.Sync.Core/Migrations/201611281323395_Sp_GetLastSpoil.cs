namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sp_GetLastSpoil : DbMigration
    {
        private readonly string dboName = "[Sync].[Sp_GetLastSpoil]";
        public override void Up()
        {
            String tsql = @"SELECT TOP 1 * 
                            FROM [Sync].[Spoils] trt
                           ORDER BY trt.RefModifiedDate DESC, trt.RefCreatedDate DESC";

            CreateStoredProcedure(dboName, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(dboName);
        }
    }
}