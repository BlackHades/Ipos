using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class auto : DbMigration
    {
        public override void Up()
        {
            var sql = @" UPDATE dbo.Audit
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.BulkSMS
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    							WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE [dbo].[Order]
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.Customer
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.OrderDetails
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.Product
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.Category
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL
UPDATE dbo.Spoil
                                SET IsDeleted=0,
    	                        CreatedBy_Id = 1
    	                        WHERE IsDeleted  IS NULL
    	                        AND CreatedBy_Id IS NULL";
        }

        public override void Down()
        {

        }
    }
}