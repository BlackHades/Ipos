using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetOrderedProducts : DbMigration
    {
        readonly string sp_name = "[dbo].[Sp_GetOrderedProducts]";
        public override void Up()
        {
            var tsqlBody = @"SELECT (SELECT name from product where productId = od.Product_Id) ProductName,
    	 od.Quantiy quantity, od.product_id productid, od.discount,
    	
		 od.CostPrice,
		 (od.price
    	  / 
    	  Case when od.quantiy > 0 
    	   then od.quantiy 
    		 else 1 
    		 end ) Price, 
    	   od.Order_UId OrderUId,
    	 od.OrderDetailUId OrderDetailId,ord.IsDiscounted,
    	 ord.PaymentMethod,ord.OrderStatus [Status],
    ord.Total TotalPrice ,ISNULL( c.firstName + c.LastName,'NA') as customerName,
    	 CONVERT(nvarchar(12), ord.EntryDate, 13) as CreatedDate
    	FROM OrderDetails od
    	JOIN [dbo].[Order] ord ON ord.OrderUId = od.[Order_UId]
    	LEFT JOIN customer c ON c.[CustomerId]=ord.[Customer_UId]
    	WHERE ord.[OrderUId] = @orderUId";

            CreateStoredProcedure(sp_name, t => new
            {
                OrderUId = t.Guid()
            }, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
