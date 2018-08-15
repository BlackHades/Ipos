using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Sp_SearchCategories : DbMigration
    {
        readonly string sp_name = "Sp_SearchCategories";
        public override void Up()
        {
            var tsqlBody = @"SELECT cat.CategoryUId, cat.Name, cat.[Description],  COUNT(*) OVER () as TotalCount,
                                (SELECT count (*) from product where category_Uid = cat.CategoryUId) ProductCount
    		                        FROM [Category] cat 
                                     
                                     WHERE (@Keyword IS NULL OR cat.[Name] like '%'+ @keyword + '%') and cat.isdeleted = 0
				                     ORDER BY  ModifiedOnUtc, EntryDate DESC 
    			                     OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY";

            CreateStoredProcedure(sp_name, t => new
            {
                PageIndex = t.Int(defaultValue: 1),
                PageSize = t.Int(defaultValue: 50),
                keyword = t.String(maxLength: 50)
            }, tsqlBody);
        }


        public override void Down()
        {
            DropStoredProcedure(sp_name);
        }
    }
}
