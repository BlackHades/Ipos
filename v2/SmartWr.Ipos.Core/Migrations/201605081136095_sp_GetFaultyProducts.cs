using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class sp_GetFaultyProducts : DbMigration
    {
        readonly string spName = "[dbo].[Sp_GetFaultyProducts]";
        public override void Up()
        {
            var tsqlBody = @"
	                SET NOCOUNT ON;
                    SELECT TOP 10 sp.Quantity, sp.SpoilId, sp.EntryDate , prd.Name FROM [Spoil] sp
                    LEFT JOIN [Product] prd ON sp.Product_Id = prd.ProductId
                    ORDER BY sp.EntryDate DESC";

            CreateStoredProcedure(spName, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}