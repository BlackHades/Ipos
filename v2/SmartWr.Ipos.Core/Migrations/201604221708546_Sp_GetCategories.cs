using System;
using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetCategories : DbMigration
    {
        private readonly string sp_name = "[dbo].[Sp_GetCategories]";
        public override void Up()
        {
            String sqlBody = @"WITH dbResult AS (SELECT c.CategoryUId, c.Name , [Description] , Row_Number() over(order by entrydate) RowNumber,
								Count(*) over() as TotalCount,
                               (SELECT count (*) from product where category_Uid= c.CategoryUId) ProductCount FROM CATEGORY c)
							    Select * from dbResult Where RowNumber between  (@itemsPerPage * (@pageIndex - 1)) +1   and (@pageIndex * @itemsPerPage)";

           

            CreateStoredProcedure(sp_name,
            t => new
            {
                pageIndex = t.Int(defaultValue: 1),
                itemsPerPage = t.Int(defaultValue: 10)
            },
            sqlBody);
        }




        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
