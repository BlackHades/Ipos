using System;
using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetDetailsOfOrder : DbMigration
    {
        private readonly string sp_name = "[dbo].[Sp_GetDetailsOfOrder]";
        public override void Up()
        {
            String sqlBody = @"SELECT p.Name ProductName, ordt.Quantiy ItemCount, p.Price, ordt.Discount
                                FROM OrderDetails ordt
                                LEFT JOIN Product p
                                ON ordt.Product_Id = p.ProductId
                                WHERE ordt.Order_UId = @OrderId";
            CreateStoredProcedure(sp_name,
                t => new
                {
                    OrderId = t.Guid()
                },
                sqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
