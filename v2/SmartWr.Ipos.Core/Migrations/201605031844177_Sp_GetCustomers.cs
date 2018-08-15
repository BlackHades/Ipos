using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetCustomers : DbMigration
    {
        private readonly string SpName = "GetCustomers";

        public override void Up()
        {
            var tsqlBody = @";WITH dbResult AS (SELECT  ROW_NUMBER() OVER(ORDER BY CustomerId) RowNumber,
			COUNT(CustomerId) OVER() Total, * FROM Customer c)
					SELECT * FROM dbResult
			WHERE RowNumber BETWEEN (@ItemsPerPage *(@PageIndex - 1)) +1 AND  (@ItemsPerPage * @PageIndex)
";
            CreateStoredProcedure(SpName, t => new
            {
                PageIndex = t.Int(1),
                ItemsPerPage = t.Int(1)
            }, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(SpName);
        }
    }
}