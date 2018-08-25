namespace SmartWr.Ipos.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSubCategoryFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "ParentCatId", c => c.Int());
            CreateIndex("dbo.Category", "ParentCatId");
            AddForeignKey("dbo.Category", "ParentCatId", "dbo.Category", "CategoryUId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Category", "ParentCatId", "dbo.Category");
            DropIndex("dbo.Category", new[] { "ParentCatId" });
            DropColumn("dbo.Category", "ParentCatId");
        }
    }
}
