namespace Ipos.Sync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Sp_GetLastTransaction : DbMigration
    {
        public override void Up()
        {
            String tsql = @"SELECT TOP 1 * 
                            FROM [Sync].[Transactions] trt
                           ORDER BY trt.RefModifiedDate DESC, trt.RefCreatedDate DESC";

            CreateStoredProcedure("[Sync].[Sp_GetLastTransaction]", tsql);
        }

        public override void Down()
        {
            DropStoredProcedure("[Sync].[Sp_GetLastTransaction]");
        }
    }
}
