using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetRecentItemsBought : DbMigration
    {
        readonly string spName = "[dbo].[Sp_GetRecentBoughtItems]";
        public override void Up()
        {
            var tsqlBody = @"
                            
                                SELECT TOP 10 ord.OrderUId,
	                             (SELECT SUM (ISNULL(od.quantiy,0)) 
    					                             FROM OrderDetails od
    					                             WHERE od.Order_UId = ord.OrderUId) ItemCount,
	                             usr.UserName StaffName, 
	                             ISNULL(ord.Total,0) TotalSellingPrice,
	                             ord.EntryDate TransactionDate
	                             FROM [Order] ord
                                 LEFT JOIN [dbo].[aspnet_Users] usr
                                 ON ord.[User_Id] =  usr.UserId
	                             ORDER BY ord.EntryDate DESC";
            CreateStoredProcedure(spName, tsqlBody);
        }

        public override void Down() 
        {
            DropStoredProcedure(spName);
        }
    }
}