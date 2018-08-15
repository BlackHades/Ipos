using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetWastedItems : DbMigration
    {
        readonly string spName = "GetWastedItems";
        public override void Up()
        {
            var tsqlBody = @";WITH dbresult AS (
					SELECT  ROW_NUMBER() OVER(ORDER BY sp.entrydate DESC) RowNumber,
					COUNT(*) over () as TotalCount,
					 sp.title, sp.entrydate, sp.[Description], sp.SpoilId, sp.quantity, p.Name as productName,users.UserName
					 FROM Spoil sp
					 join product p on sp.product_Id = p.productId
					 join aspnet_Users users on users.UserId = sp.User_Id
                     where users.UserName= ISNULL(@user,users.UserName)
				  ) 
					SELECT * FROM dbresult 
					 Where dbresult.RowNumber between (@itemPerPage * (@pageIndex-1)) +1 And (@pageIndex * @itemPerPage) ";

            CreateStoredProcedure(spName, t => new
            {
                pageIndex = t.Int(defaultValue: 1),
                itemPerPage = t.Int(defaultValue: 50),
                user = t.String(defaultValue: null)
            }, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}