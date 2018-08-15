using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_IgnoreItem : DbMigration
    {
        private readonly string spName = "Sp_IgnoreItem";
        public override void Up()
        {
            var tsql = @"UPDATE [Product]  SET ReorderLevel = 0 WHERE ProductId = @ProductId 
	 SELECT TOP 10 prd.Name, prd.ProductId,prd.[Description], prd.EntryDate, prd.Quantity, prd.ReorderLevel FROM Product prd WHERE prd.Quantity > prd.ReorderLevel AND prd.ReorderLevel != 0 ORDER BY prd.EntryDate DESC";

            CreateStoredProcedure(spName, t => new
            {
                ProductId = t.Long()
            }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}