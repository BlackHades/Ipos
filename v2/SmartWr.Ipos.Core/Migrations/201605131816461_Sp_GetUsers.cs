using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_GetUsers : DbMigration
    {
        private readonly string spName = "[dbo].[GetUsers]";
        public override void Up()
        {
            var tsql = @"
                                        WITH dbResult AS ( SELECT ROW_NUMBER() OVER(ORDER BY id DESC) as RowNumber, users.UserName ,users.Id, users.LockoutEnabled,
                                        users.email,users.firstName	,users.lastname,users.PhoneNumber , COUNT(*) OVER () AS TotalCount FROM  AspnetUsers users
                                            WHERE @q IS NULL OR users.username LIKE '%' + @q+'%' or users.firstname LIKE '%' + @q+'%' 
	                                          or users.lastname LIKE '%' + @q+'%'  or users.phoneNumber LIKE '%' + @q+'%' 
	                                          or email LIKE '%' + @q+'%'
                                         )
                                          SELECT * FROM dbResult
                                            WHERE dbresult.RowNumber BETWEEN (@page_size * (@page_number-1)) +1 AND (@page_size * @page_number)";
            CreateStoredProcedure(spName, t => new
            {
                page_number = t.Int(defaultValueSql: "1"),
                page_size = t.Int(defaultValueSql: "50"),
                @q = t.String(defaultValueSql: "NULL")
            }, tsql);
        }

        public override void Down()
        {
            DropStoredProcedure(spName);
        }
    }
}
