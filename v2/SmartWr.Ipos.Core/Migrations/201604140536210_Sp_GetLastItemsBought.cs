using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetLastItemsBought : DbMigration
    {
        private readonly string sp_name = "[dbo].[Sp_GetLastItemsBought]";

        public override void Up()
        {
            string sqlbody = @"SELECT TOP 20 
                                    ord.OrderUId,
                                    COUNT(ord.OrderUId) ItemCount,
                                    usr.UserName StaffName, 
                                    ISNULL(SUM(ord.Total),0) TotalSellingPrice,
                                    ord.EntryDate TransactionDate
                                FROM [Order] ord
                                LEFT JOIN OrderDetails ordt
                                ON ord.OrderUId = ordt.Order_UId
                                LEFT JOIN [dbo].[aspnet_Users] usr
                                ON ord.[User_Id] =  usr.UserId
                                GROUP BY  ord.OrderUId, ord.EntryDate, usr.UserName, ord.Total
                                ORDER BY ord.EntryDate DESC";
            CreateStoredProcedure(sp_name, sqlbody);
        }

        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
