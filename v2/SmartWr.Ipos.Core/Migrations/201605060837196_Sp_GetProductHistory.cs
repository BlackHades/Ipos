using System;
using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetProductHistory : DbMigration
    {
        private readonly string sp_name = "[dbo].[Sp_GetProductHistory]";
        public override void Up()
        {
            String sqlBody = @"  WITH dbResult AS (SELECT Count(*) over() as Total, od.Price/od.Quantiy as Price, od.Discount,
    	od.Quantiy as quantity,convert(nvarchar(12),od.entryDate,9) CreatedDate , od.Price as TotalPrice,
    	Row_Number() over(order by entrydate desc) RowNumber
    	 FROM orderdetails od 
    	where od.Product_Id=@id)
    
    	   Select * from dbResult Where RowNumber between  (@itemsPerPage * (@pageIndex - 1)) +1   and (@pageIndex * @itemsPerPage)";
        
             CreateStoredProcedure(sp_name,
            t => new
            {
                 pageIndex = t.Int(defaultValue: 1),
                itemsPerPage = t.Int(defaultValue: 50),
                id = t.Int(defaultValue: null)
            },
            sqlBody);
        }
        
        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
