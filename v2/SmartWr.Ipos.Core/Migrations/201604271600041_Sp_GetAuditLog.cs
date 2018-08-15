using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetAuditLog : DbMigration
    {
        readonly string spName = "[dbo].[GetAuditLog]";
        public override void Up()
        {
            var tsqlBody = @"WITH queryResult AS (
							SELECT ROW_NUMBER() OVER(ORDER BY adt.entrydate DESC) RowNumber,
    							COUNT(*) OVER () AS TotalCount, users.UserName, adt.[description],
								adt.AuditId,adt.AuditType,adt.EntryDate FROM Audit adt
    							 JOIN aspnet_Users users ON users.UserId = adt.User_Id
    					         WHERE users.UserName = ISNULL(@user,users.UserName) 
    							 AND  (adt.AuditType IS NULL OR adt.AuditType = ISNULL(@type,adt.AuditType))
								 AND adt.IsDeleted = 0
						 )
                SELECT * FROM queryResult
		        Where RowNumber BETWEEN (@itemPerPage * (@pageIndex-1)) +1 AND (@pageIndex * @itemPerPage)  ";

            CreateStoredProcedure(spName, t => new
            {
                @pageIndex = t.Int(1),
                itemPerPage = t.Int(1),
                @user = t.String(defaultValue: "NULL"),
                @type = t.Int(null)
            }, tsqlBody);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}