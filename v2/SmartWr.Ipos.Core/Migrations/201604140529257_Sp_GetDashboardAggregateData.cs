using System;
using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetDashboardAggregateData : DbMigration
    {
        private readonly string sp_name = "[dbo].[Sp_GetDashboardAggregateData]";

        public override void Up()
        {
            String sqlBody = @"SET DATEFORMAT dmy
                 SELECT ISNULL((Select Count(*)
    	                FROM [Order] od
    	                WHERE DAY(@SearchDate) = DAY(od.EntryDate) AND
                    Year(@SearchDate) = YEAR(od.EntryDate) 
                    AND Month(@SearchDate) = Month(od.EntryDate)
                    ), 0) TotalTransactionCount,

                 ISNULL((Select SUM(od.Total)
    	                FROM [Order] od
    	                WHERE DAY(@SearchDate) = DAY(od.EntryDate) AND
                    Year(@SearchDate) = YEAR(od.EntryDate) 
                    AND Month(@SearchDate) = Month(od.EntryDate)
                    ), 0) TotalTodaySales,
                ISNULL((
    	                Select SUM(od.Total)
    	                FROM [Order] od 	
    	                WHERE @SearchDate >= DATEADD(wk, DATEDIFF(wk,0, od.EntryDate), -1) 
    	                AND @SearchDate <= DATEADD(wk, DATEDIFF(wk,0, od.EntryDate), 5)
                    ), 0) TotalWeekSales";

            CreateStoredProcedure(sp_name
                , t => new
                {
                    SearchDate = t.String()
                }, sqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
