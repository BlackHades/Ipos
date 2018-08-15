using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetRestockItems : DbMigration
    {
        readonly string spName = "Sp_GetRestockItems";
        public override void Up()
        {
            var tsqlBody = @"SET NOCOUNT ON;
                             SELECT TOP 10 prd.Name, prd.ProductId,Prd.ProductUId,prd.[Description], prd.EntryDate, prd.Quantity, prd.ReorderLevel FROM Product prd WHERE prd.Quantity < prd.ReorderLevel AND prd.ReorderLevel != 0 ORDER BY prd.EntryDate DESC ";
            CreateStoredProcedure(spName, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}