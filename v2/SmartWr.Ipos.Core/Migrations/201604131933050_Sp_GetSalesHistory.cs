using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetSalesHistory : DbMigration
    {
        private readonly string sp_name = "Sp_GetSalesHistory";

        public override void Up()
        {
            var tsql = @" 
                       SET DATEFORMAT dmy
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

            CreateStoredProcedure(sp_name, t => new
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

        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}