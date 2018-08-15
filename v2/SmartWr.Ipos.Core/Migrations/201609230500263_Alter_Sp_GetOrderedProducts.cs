namespace SmartWr.Ipos.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Alter_Sp_GetOrderedProducts : DbMigration
    {
        readonly string sp_name = "[dbo].[Sp_GetOrderedProducts]";
        public override void Up()
        {
            var tsqlBody = @"SELECT prod.Name ProductName,
    	                        prod.[Description],
                                ISNULL(od.Quantiy, 1) Quantity,
    	                        od.Product_Id ProductId, 
    	                        ISNULL(od.Discount, 0) Discount,
    	                        od.CostPrice CostPrice,
    	                        ISNULL(prod.Price,0) CurrentPrice,
    	                        ISNULL(od.Price, 0) UnitCost,
    	                        (ISNULL(od.Price, 0) - ISNULL(od.Discount, 0)) Price,
    	                        (ISNULL(od.Price, 0) - ISNULL(od.Discount, 0)) Total,
    	                        od.Order_UId OrderUId,
    	                        od.OrderDetailUId OrderDetailId,
    	                        ord.IsDiscounted,
    	                        ord.PaymentMethod,ord.OrderStatus [Status],
    	                        ord.Total TotalPrice,
    		                        ord.Remark Remark,
    	                        ISNULL( c.FirstName + c.LastName,'NA') AS CustomerName,
    	                        CONVERT(NVARCHAR(12), ord.EntryDate, 13) AS CreatedDate
                            FROM OrderDetails od
                            JOIN [dbo].[Product] prod 
                            ON od.Product_Id = prod.[ProductId]
                            LEFT JOIN [Order] ord
                            ON od.Order_UId = ord.OrderUId
                            LEFT JOIN Customer c 
                            ON c.[CustomerId]= ord.[Customer_UId]
                            WHERE ord.[OrderUId] = @orderUId";

            AlterStoredProcedure(sp_name, t => new
            {
                OrderUId = t.Guid()
            }, tsqlBody);
        }

        public override void Down()
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

            AlterStoredProcedure(sp_name, t => new
            {
                OrderUId = t.Guid()
            }, tsqlBody);
        }
    }
}
