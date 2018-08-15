namespace SmartWr.Ipos.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Alter_Sp_SearchProducts : DbMigration
    {
        readonly string sp_name = "Sp_SearchProducts";

        public override void Up()
        {
            var tsqlBody = @"
			  DECLARE @convertedDate VARCHAR = NULL;

				  WITH dbResult
					   AS (SELECT Row_number()
									OVER (ORDER BY p.ModifiedOnUtc, p.EntryDate DESC) RowNumber,
								  Count(*) OVER() TotalCount,
								  p.ProductId,
								  p.Barcode,
								  p.CanExpire,
								  p.ExpiryDate,
								  p.Name,
                                  p.Description,
								  p.Quantity,
								  p.IsDiscountable,
								  p.Price,
								  p.ProductUId
						   FROM   Product p
								  LEFT JOIN category cat
										 ON cat.CategoryUId = p.Category_UId
						   WHERE  (( @query IS NULL
									 OR p.NAME LIKE '%' + @query + '%'
									 OR p.Description LIKE + '%' + @query + '%'
									 OR p.ProductId LIKE @query
									 OR p.Barcode LIKE + '%' + @query + '%'
									 OR p.Price LIKE @query
									 OR p.CostPrice LIKE @query
									 OR @query IS NULL
									 OR cat.NAME = @query )
								   OR ( @query IS NULL
										 OR cat.NAME = @query ))
									  AND p.isdeleted = 0)
				  SELECT *
				  FROM   dbResult
				  WHERE  RowNumber BETWEEN ( @ItemsPerPage * ( @Page - 1 ) ) + 1 AND ( @ItemsPerPage * @Page )";
            AlterStoredProcedure(sp_name, t => new
            {
                Page = t.Int(defaultValue: 1),
                ItemsPerPage = t.Int(defaultValue: 50),
                query = t.String(maxLength: 50)
            }, tsqlBody);
        }

        public override void Down()
        {
            var tsqlBody = @"
			  DECLARE @convertedDate VARCHAR = NULL;

				  WITH dbResult
					   AS (SELECT Row_number()
									OVER (
									  ORDER BY p.ModifiedOnUtc, p.EntryDate DESC) RowNumber,
								  Count(*)
									OVER()                                        TotalCount,
								  p.ProductId,
								  p.Barcode,
								  p.CanExpire,
								  p.ExpiryDate,
								  p.NAME,
								  p.Quantity,
								  p.IsDiscountable,
								  p.Price,
								  p.ProductUId
						   FROM   Product p
								  LEFT JOIN category cat
										 ON cat.CategoryUId = p.Category_UId
						   WHERE  (( @query IS NULL
									 OR p.NAME LIKE '%' + @query + '%'
									 OR p.Description LIKE + '%' + @query + '%'
									 OR p.ProductId LIKE @query
									 OR p.Barcode LIKE + '%' + @query + '%'
									 OR p.Price LIKE @query
									 OR p.CostPrice LIKE @query
									 OR @query IS NULL
									 OR cat.NAME = @query )
								    OR (@query IS NULL
										 OR cat.NAME = @query ))
									  AND p.isdeleted = 0)
				  SELECT *
				  FROM   dbResult
				  WHERE  RowNumber BETWEEN ( @ItemsPerPage * ( @Page - 1 ) ) + 1 AND ( @ItemsPerPage * @Page )";
            AlterStoredProcedure(sp_name, t => new
            {
                Page = t.Int(defaultValue: 1),
                ItemsPerPage = t.Int(defaultValue: 50),
                query = t.String(maxLength: 50)
            }, tsqlBody);
        }
    }
}
