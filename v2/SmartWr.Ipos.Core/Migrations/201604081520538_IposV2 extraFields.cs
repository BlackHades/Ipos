namespace SmartWr.Ipos.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class IposV2extraFields : DbMigration
    {
        public override void Up()
        {
            //Audit
            AddColumn("dbo.Audit", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Audit", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Audit", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Audit", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Audit", "CreatedBy_Id");
            CreateIndex("dbo.Audit", "ModifiedBy_Id");
            AddForeignKey("dbo.Audit", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Audit", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //BulkSMS
            AddColumn("dbo.BulkSMS", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.BulkSMS", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.BulkSMS", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.BulkSMS", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.BulkSMS", "CreatedBy_Id");
            CreateIndex("dbo.BulkSMS", "ModifiedBy_Id");
            AddForeignKey("dbo.BulkSMS", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.BulkSMS", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //Order
            AddColumn("dbo.Order", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Order", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Order", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Order", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Order", "CreatedBy_Id");
            CreateIndex("dbo.Order", "ModifiedBy_Id");
            AddForeignKey("dbo.Order", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Order", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //Customer
            AddColumn("dbo.Customer", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Customer", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Customer", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Customer", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Customer", "CreatedBy_Id");
            CreateIndex("dbo.Customer", "ModifiedBy_Id");
            AddForeignKey("dbo.Customer", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Customer", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //OrderDetails
            AddColumn("dbo.OrderDetails", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.OrderDetails", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.OrderDetails", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.OrderDetails", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.OrderDetails", "CreatedBy_Id");
            CreateIndex("dbo.OrderDetails", "ModifiedBy_Id");
            AddForeignKey("dbo.OrderDetails", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrderDetails", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //Product
            AddColumn("dbo.Product", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Product", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Product", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Product", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Product", "CreatedBy_Id");
            CreateIndex("dbo.Product", "ModifiedBy_Id");
            AddForeignKey("dbo.Product", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Product", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //Category
            AddColumn("dbo.Category", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Category", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Category", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Category", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Category", "CreatedBy_Id");
            CreateIndex("dbo.Category", "ModifiedBy_Id");
            AddForeignKey("dbo.Category", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Category", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");

            //Spoil
            AddColumn("dbo.Spoil", "CreatedBy_Id", c => c.Int(defaultValue: 1));
            AddColumn("dbo.Spoil", "ModifiedBy_Id", c => c.Int());
            AddColumn("dbo.Spoil", "ModifiedOnUtc", c => c.DateTime());
            AddColumn("dbo.Spoil", "IsDeleted", c => c.Boolean( defaultValue: false));
            CreateIndex("dbo.Spoil", "CreatedBy_Id");
            CreateIndex("dbo.Spoil", "ModifiedBy_Id");
            AddForeignKey("dbo.Spoil", "CreatedBy_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Spoil", "ModifiedBy_Id", "dbo.AspNetUsers", "Id");
        }

        public override void Down()
        {
            //Spoil
            DropForeignKey("dbo.Spoil", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Spoil", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Spoil", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Spoil", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Spoil", "IsDeleted");
            DropColumn("dbo.Spoil", "ModifiedOnUtc");
            DropColumn("dbo.Spoil", "ModifiedBy_Id");
            DropColumn("dbo.Spoil", "CreatedBy_Id");

            //Category
            DropForeignKey("dbo.Category", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Category", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Category", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Category", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Category", "IsDeleted");
            DropColumn("dbo.Category", "ModifiedOnUtc");
            DropColumn("dbo.Category", "ModifiedBy_Id");
            DropColumn("dbo.Category", "CreatedBy_Id");

            //Product
            DropForeignKey("dbo.Product", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Product", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Product", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Product", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Product", "IsDeleted");
            DropColumn("dbo.Product", "ModifiedOnUtc");
            DropColumn("dbo.Product", "ModifiedBy_Id");
            DropColumn("dbo.Product", "CreatedBy_Id");

            //OrderDetails
            DropForeignKey("dbo.OrderDetails", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderDetails", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.OrderDetails", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.OrderDetails", new[] { "CreatedBy_Id" });
            DropColumn("dbo.OrderDetails", "IsDeleted");
            DropColumn("dbo.OrderDetails", "ModifiedOnUtc");
            DropColumn("dbo.OrderDetails", "ModifiedBy_Id");
            DropColumn("dbo.OrderDetails", "CreatedBy_Id");

            //Customer
            DropForeignKey("dbo.Customer", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Customer", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Customer", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Customer", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Customer", "IsDeleted");
            DropColumn("dbo.Customer", "ModifiedOnUtc");
            DropColumn("dbo.Customer", "ModifiedBy_Id");
            DropColumn("dbo.Customer", "CreatedBy_Id");

            //Order
            DropForeignKey("dbo.Order", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Order", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Order", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Order", "IsDeleted");
            DropColumn("dbo.Order", "ModifiedOnUtc");
            DropColumn("dbo.Order", "ModifiedBy_Id");
            DropColumn("dbo.Order", "CreatedBy_Id");

            //BulkSMS
            DropForeignKey("dbo.BulkSMS", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BulkSMS", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.BulkSMS", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.BulkSMS", new[] { "CreatedBy_Id" });
            DropColumn("dbo.BulkSMS", "IsDeleted");
            DropColumn("dbo.BulkSMS", "ModifiedOnUtc");
            DropColumn("dbo.BulkSMS", "ModifiedBy_Id");
            DropColumn("dbo.BulkSMS", "CreatedBy_Id");

            //Audit
            DropForeignKey("dbo.Audit", "ModifiedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Audit", "CreatedBy_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Audit", new[] { "ModifiedBy_Id" });
            DropIndex("dbo.Audit", new[] { "CreatedBy_Id" });
            DropColumn("dbo.Audit", "IsDeleted");
            DropColumn("dbo.Audit", "ModifiedOnUtc");
            DropColumn("dbo.Audit", "ModifiedBy_Id");
            DropColumn("dbo.Audit", "CreatedBy_Id");
        }
    }
}