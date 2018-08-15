namespace SmartWr.Ipos.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Alter_Sp_GetSalesHistory : DbMigration
    {
        private readonly string sp_name = "Sp_GetSalesHistory";
        public override void Up()
        {
            var tsql = @"SET DATEFORMAT dmy
    
                        DECLARE @startDate DATE = NULL;
                        DECLARE @endDate DATE = NULL;
    
                        IF ISDATE(@sDate) = 1
		                    SET @startDate = Convert(DATE, @sDate)
                        IF ISDATE(@eDate) = 1
		                    SET @endDate = Convert(DATE, @eDate);
    
                       SELECT ROW_NUMBER() OVER(ORDER BY ord.entrydate DESC) AS RowNumber, 
    	                        COUNT(*) OVER () AS TotalCount,
                                ord.OrderStatus [Status],
                                ord.PaymentMethod,
	                            ISNULL(SUM(od.Discount), 0) Discount,
                                (SUM(od.Price)) - SUM(od.Discount) AS Total,
                                od.Order_UId,
	                            (CASE WHEN SUM(od.Price) > 0 
    	                            THEN SUM((od.Price) - (od.costPrice * od.Quantiy))
	                                WHEN SUM((prod.Price * od.Quantiy) + (od.Price) - (od.costPrice * od.Quantiy)) > 0
		                            THEN SUM((prod.Price * od.Quantiy) + (od.Price) - (od.costPrice * od.Quantiy))
	                            ELSE
		                            0
	                            END) Profit,
                                SUM (ISNULL(od.quantiy,0)) AS TotalItemsBought,
                                users.username staffName,
                                CONVERT(NVARCHAR(12), ord.EntryDate, 13) as CreatedDate
                        FROM OrderDetails od
                        LEFT JOIN [dbo].[Order] ord
                        ON od.Order_UId = ord.OrderUId
                        LEFT JOIN aspnet_Users users
                        On ord.User_Id = users.UserId
	                    LEFT JOIN Product prod
	                    ON od.Product_Id = prod.ProductId
                        WHERE Convert(DATE,ord.EntryDate) >= ISNULL( @startDate,  Convert(DATE,ord.EntryDate))
                        AND Convert(DATE,ord.EntryDate) <= ISNULL( @endDate,  Convert(DATE,ord.EntryDate))
                        AND ord.orderstatus = ISNULL(@status, ord.orderstatus)
                        AND (@TransactionId is NULL or CONVERT (NVARCHAR(50),orderUID) like '%' +  @TransactionId)
                        AND (@StockId IS NULL OR od.Product_Id = @StockId)
                        GROUP BY od.Order_UId, ord.EntryDate, ord.OrderStatus,
	                    ord.PaymentMethod, ord.Total, users.UserName
                        ORDER BY ord.EntryDate DESC
                        OFFSET @page_number ROWS FETCH NEXT @page_size ROWS ONLY";

            AlterStoredProcedure(sp_name, t => new
            {
                page_number = t.Int(defaultValue: 0),
                page_size = t.Int(defaultValue: 50),
                sDate = t.String(defaultValue: null, unicode: false),
                eDate = t.String(defaultValue: null, unicode: false),
                user = t.String(defaultValue: null, maxLength: 50),
                status = t.Int(defaultValue: null),
                TransactionId = t.String(defaultValue: null, maxLength: 50),
                StockId = t.Int(defaultValue: null)
            }, tsql);
        }

        public override void Down()
        {
            var tsql = @"SET DATEFORMAT dmy
    	                        declare @sStartDate date = null, 
    	                        @eEndDate date = null;
    
                        if ISDATE(@StartDate) = 1
                        set @sStartDate = convert(date, @StartDate)
    
                        if ISDATE(@endDate) = 1
                        set @eEndDate = CONVERT(date, @endDate)
    
                        ;with dbresult as (
    						                       Select ROW_NUMBER() Over(order by entrydate desc) as RowNumber, 
    				                            COUNT(*) over () as TotalCount,
    				                             ord.OrderStatus [Status],
    				                             ord.PaymentMethod,
    				                             ord.Total,
    				                             ord.OrderUId,
    				                             ( 
    			                                  SELECT ( SUM ((od.Price) - (od.costPrice * od.Quantiy) ) )
    					                             from OrderDetails od
    					                             where od.Order_UId = ord.OrderUId
    						                              and od.CostPrice > 0 and od.Price > 0
    				                             ) as Profit,
    				                             (
    				 	                            select SUM (isnull(od.quantiy,0)) 
    					                             from OrderDetails od
    					                             where od.Order_UId = ord.OrderUId
    				                             ) as TotalItemsBought,

												 SUM (isnull(ord.Total,0)) over() as SumTotal,
												
												users.username staffName,

    				                            CONVERT(nvarchar(12), ord.EntryDate, 13) as CreatedDate
    				                            
												from [dbo].[Order] ord
												JOIN aspnet_Users users
												On users.UserId = ord.User_Id
												and users.UserName = ISNULL(@user,users.UserName)

    				                            where (ord.EntryDate >= ISNULL( @sStartDate, ord.EntryDate)
    				                            and convert(date, ord.EntryDate) <= ISNULL( @eEndDate, ord.EntryDate) )
    				                            and ord.orderstatus = ISNULL(@status, ord.orderstatus)
    				                            and @TransactionId is null or convert (nvarchar(50),orderUID) like '%' +  @TransactionId   
												)
    	                            Select dbresult.* from  dbresult
    	                            Where dbresult.RowNumber between (@page_size * (@page_number-1)) +1 And (@page_size * @page_number) ";

            AlterStoredProcedure(sp_name, t => new
            {
                page_number = t.Int(defaultValue: 1),
                page_size = t.Int(defaultValue: 50),
                startDate = t.DateTime(),
                endDate = t.DateTime(),
                user = t.String(defaultValue: null, maxLength: 50),
                status = t.Int(defaultValue: null),
                TransactionId = t.String(defaultValue: null, maxLength: 50)
            }, tsql);
        }
    }
}
