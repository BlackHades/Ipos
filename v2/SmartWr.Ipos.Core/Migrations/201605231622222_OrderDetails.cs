using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class OrderDetails : DbMigration
    {
        private readonly string spName = "dbo.Sp_GetOrderDetails";
        public override void Up()
        {
            var tsql = @"
SET NOCOUNT ON;

SELECT prd.Name ProductName, usr.UserName Staffname,
		  ordt.Price, ordt.Quantiy Quantity, ordt.CostPrice, ordt.Discount Discount
		   FROM [Order] ord 
		  LEFT JOIN [OrderDetails] ordt ON ord.OrderUId = ordt.Order_UId 
		  LEFT JOIN [Product] prd ON ordt.Product_Id = prd.ProductId 
		  LEFT JOIN [aspnet_Users] usr ON ord.[User_Id] =  usr.UserId  
		  WHERE ord.OrderUId = @orderuid;
";
            CreateStoredProcedure(spName, t => new
            {
                orderuid = t.Guid()
            }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}