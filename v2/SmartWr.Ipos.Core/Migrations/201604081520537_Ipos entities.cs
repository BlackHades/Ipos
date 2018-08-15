using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class Iposentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.aspnet_Applications",
                c => new
                    {
                        ApplicationId = c.Guid(nullable: false),
                        ApplicationName = c.String(nullable: false, maxLength: 256),
                        LoweredApplicationName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ApplicationId);

            CreateTable(
                "dbo.aspnet_Membership",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        Password = c.String(nullable: false, maxLength: 128),
                        PasswordFormat = c.Int(nullable: false),
                        PasswordSalt = c.String(nullable: false, maxLength: 128),
                        MobilePIN = c.String(maxLength: 16),
                        Email = c.String(maxLength: 256),
                        LoweredEmail = c.String(maxLength: 256),
                        PasswordQuestion = c.String(maxLength: 256),
                        PasswordAnswer = c.String(maxLength: 128),
                        IsApproved = c.Boolean(nullable: false),
                        IsLockedOut = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        LastPasswordChangedDate = c.DateTime(nullable: false),
                        LastLockoutDate = c.DateTime(nullable: false),
                        FailedPasswordAttemptCount = c.Int(nullable: false),
                        FailedPasswordAttemptWindowStart = c.DateTime(nullable: false),
                        FailedPasswordAnswerAttemptCount = c.Int(nullable: false),
                        FailedPasswordAnswerAttemptWindowStart = c.DateTime(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.aspnet_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.aspnet_Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ApplicationId);

            CreateTable(
                "dbo.aspnet_Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        LoweredUserName = c.String(nullable: false, maxLength: 256),
                        MobileAlias = c.String(maxLength: 16),
                        IsAnonymous = c.Boolean(nullable: false),
                        LastActivityDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.aspnet_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);

            CreateTable(
                "dbo.aspnet_PersonalizationPerUser",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PathId = c.Guid(),
                        UserId = c.Guid(),
                        PageSettings = c.Binary(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.aspnet_Paths", t => t.PathId)
                .ForeignKey("dbo.aspnet_Users", t => t.UserId)
                .Index(t => t.PathId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.aspnet_Paths",
                c => new
                    {
                        PathId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        Path = c.String(nullable: false, maxLength: 256),
                        LoweredPath = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.PathId)
                .ForeignKey("dbo.aspnet_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);

            CreateTable(
                "dbo.aspnet_PersonalizationAllUsers",
                c => new
                    {
                        PathId = c.Guid(nullable: false),
                        PageSettings = c.Binary(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PathId)
                .ForeignKey("dbo.aspnet_Paths", t => t.PathId)
                .Index(t => t.PathId);

            CreateTable(
                "dbo.aspnet_Profile",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        PropertyNames = c.String(nullable: false),
                        PropertyValuesString = c.String(nullable: false),
                        PropertyValuesBinary = c.Binary(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.aspnet_Users", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.aspnet_Roles",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        RoleName = c.String(nullable: false, maxLength: 256),
                        LoweredRoleName = c.String(nullable: false, maxLength: 256),
                        Description = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.aspnet_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);

            CreateTable(
                "dbo.Audit",
                c => new
                    {
                        AuditId = c.Guid(nullable: false),
                        AuditType = c.Int(),
                        EntryDate = c.DateTime(),
                        User_Id = c.Guid(),
                        Description = c.String(maxLength: 250),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Int(nullable: true),
                    })

                .PrimaryKey(t => t.AuditId)
                .ForeignKey("dbo.aspnet_Users", t => t.User_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
                .Index(t => t.User_Id);
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.BulkSMS",
                c => new
                    {
                        SMSUId = c.Guid(nullable: false),
                        Sender = c.String(maxLength: 50),
                        Message = c.String(),
                        Recipients = c.String(),
                        EntryDate = c.DateTime(),
                        MessageType = c.Int(),
                        DeliveryStatus = c.Int(),
                        User_Id = c.Guid(nullable: false),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.SMSUId)
                .ForeignKey("dbo.aspnet_Users", t => t.User_Id, cascadeDelete: true)
                //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
                .Index(t => t.User_Id);
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderUId = c.Guid(nullable: false),
                        EntryDate = c.DateTime(),
                        User_Id = c.Guid(nullable: false),
                        IsDiscounted = c.Boolean(),
                        Total = c.Decimal(precision: 18, scale: 2),
                        Remark = c.String(maxLength: 250),
                        OrderStatus = c.Int(),
                        Customer_UId = c.Int(),
                        PaymentMethod = c.Int(),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.OrderUId)
                .ForeignKey("dbo.aspnet_Users", t => t.User_Id, cascadeDelete: true)
                //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
                .ForeignKey("dbo.Customer", t => t.Customer_UId)
                .Index(t => t.User_Id)
                .Index(t => t.Customer_UId);
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        LastName = c.String(maxLength: 150),
                        FirstName = c.String(maxLength: 150),
                        Sex = c.String(maxLength: 1, fixedLength: true),
                        Email = c.String(maxLength: 150),
                        PhoneNo = c.String(maxLength: 250),
                        EntryDate = c.DateTime(),
                        Address = c.String(maxLength: 350),
                        Remarks = c.String(maxLength: 250),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.CustomerId);
            //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
            //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailUId = c.Guid(nullable: false),
                        EntryDate = c.DateTime(),
                        Discount = c.Double(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        Quantiy = c.Int(),
                        Product_Id = c.Int(),
                        Order_UId = c.Guid(nullable: false),
                        CostPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(maxLength: 250),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.OrderDetailUId)
                //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
                .ForeignKey("dbo.Order", t => t.Order_UId)
                .ForeignKey("dbo.Product", t => t.Product_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Order_UId);
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductUId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 250),
                        Price = c.Decimal(precision: 18, scale: 2),
                        Quantity = c.Int(),
                        EntryDate = c.DateTime(),
                        Insert_UId = c.Guid(),
                        Update_UId = c.Guid(),
                        PhotoURL = c.String(maxLength: 250),
                        Extention = c.String(maxLength: 20),
                        FileName = c.String(maxLength: 150),
                        IsDiscountable = c.Boolean(nullable: false),
                        Barcode = c.String(maxLength: 250),
                        Notes = c.String(maxLength: 250),
                        CostPrice = c.Decimal(precision: 18, scale: 2),
                        ReorderLevel = c.Int(),
                        ContentType = c.String(maxLength: 50),
                        FileSize = c.Int(),
                        ExpiryDate = c.DateTime(),
                        CanExpire = c.Boolean(nullable: false),
                        Category_UId = c.Int(),
                        IsDiscontinued = c.Boolean(),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.aspnet_Users", t => t.Insert_UId)
                .ForeignKey("dbo.Category", t => t.Category_UId)
                //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
                .Index(t => t.Insert_UId)
                .Index(t => t.Category_UId);
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryUId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 250),
                        EntryDate = c.DateTime(),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.CategoryUId);
            //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
            //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.Spoil",
                c => new
                    {
                        SpoilId = c.Guid(nullable: false),
                        Title = c.String(maxLength: 150),
                        Description = c.String(maxLength: 250),
                        Product_Id = c.Int(),
                        EntryDate = c.DateTime(),
                        Quantity = c.Int(),
                        User_Id = c.Guid(),
                        //CreatedBy_Id = c.Int(defaultValue: 1),
                        //ModifiedBy_Id = c.Int(),
                        //ModifiedOnUtc = c.DateTime(),
                        //IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => t.SpoilId);
            //.ForeignKey("dbo.AspNetUsers", t => t.CreatedBy_Id)
            //.ForeignKey("dbo.AspNetUsers", t => t.ModifiedBy_Id)
            //.Index(t => t.CreatedBy_Id)
            //.Index(t => t.ModifiedBy_Id);

            CreateTable(
                "dbo.aspnet_UsersInRoles",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.aspnet_Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.aspnet_Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Product_Category",
                c => new
                    {
                        Category_UId = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Category_UId, t.Product_Id })
                .ForeignKey("dbo.Category", t => t.Category_UId, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.Product_Id, cascadeDelete: false)
                .Index(t => t.Category_UId)
                .Index(t => t.Product_Id);

        }

        public override void Down()
        {
            //DropForeignKey("dbo.Spoil", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Spoil", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.aspnet_Membership", "UserId", "dbo.aspnet_Users");
            DropForeignKey("dbo.OrderDetails", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.OrderDetails", "Order_UId", "dbo.Order");
            //DropForeignKey("dbo.Product", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Product", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Product", "Category_UId", "dbo.Category");
            DropForeignKey("dbo.Product", "Insert_UId", "dbo.aspnet_Users");
            DropForeignKey("dbo.Product_Category", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.Product_Category", "Category_UId", "dbo.Category");
            //DropForeignKey("dbo.Category", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Category", "CreatedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.OrderDetails", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.OrderDetails", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "Customer_UId", "dbo.Customer");
            DropForeignKey("dbo.Order", "User_Id", "dbo.aspnet_Users");
            //DropForeignKey("dbo.Customer", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Customer", "CreatedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Order", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Order", "CreatedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.BulkSMS", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.BulkSMS", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.BulkSMS", "User_Id", "dbo.aspnet_Users");
            DropForeignKey("dbo.Audit", "User_Id", "dbo.aspnet_Users");
            //DropForeignKey("dbo.Audit", "ModifiedBy_Id", "dbo.AspNetUsers");
            //DropForeignKey("dbo.Audit", "CreatedBy_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.aspnet_UsersInRoles", "UserId", "dbo.aspnet_Users");
            DropForeignKey("dbo.aspnet_UsersInRoles", "RoleId", "dbo.aspnet_Roles");
            DropForeignKey("dbo.aspnet_Roles", "ApplicationId", "dbo.aspnet_Applications");
            DropForeignKey("dbo.aspnet_Profile", "UserId", "dbo.aspnet_Users");
            DropForeignKey("dbo.aspnet_PersonalizationPerUser", "UserId", "dbo.aspnet_Users");
            DropForeignKey("dbo.aspnet_PersonalizationPerUser", "PathId", "dbo.aspnet_Paths");
            DropForeignKey("dbo.aspnet_PersonalizationAllUsers", "PathId", "dbo.aspnet_Paths");
            DropForeignKey("dbo.aspnet_Paths", "ApplicationId", "dbo.aspnet_Applications");
            DropForeignKey("dbo.aspnet_Users", "ApplicationId", "dbo.aspnet_Applications");
            DropForeignKey("dbo.aspnet_Membership", "ApplicationId", "dbo.aspnet_Applications");
            DropIndex("dbo.Product_Category", new[] { "Product_Id" });
            DropIndex("dbo.Product_Category", new[] { "Category_UId" });
            DropIndex("dbo.aspnet_UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.aspnet_UsersInRoles", new[] { "RoleId" });
            //DropIndex("dbo.Spoil", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Spoil", new[] { "CreatedBy_Id" });
            //DropIndex("dbo.Category", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Category", new[] { "CreatedBy_Id" });
            //DropIndex("dbo.Product", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Product", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Product", new[] { "Category_UId" });
            DropIndex("dbo.Product", new[] { "Insert_UId" });
            //DropIndex("dbo.OrderDetails", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.OrderDetails", new[] { "CreatedBy_Id" });
            DropIndex("dbo.OrderDetails", new[] { "Order_UId" });
            DropIndex("dbo.OrderDetails", new[] { "Product_Id" });
            //DropIndex("dbo.Customer", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Customer", new[] { "CreatedBy_Id" });
            //DropIndex("dbo.Order", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Order", new[] { "CreatedBy_Id" });
            DropIndex("dbo.Order", new[] { "Customer_UId" });
            DropIndex("dbo.Order", new[] { "User_Id" });
            //DropIndex("dbo.BulkSMS", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.BulkSMS", new[] { "CreatedBy_Id" });
            DropIndex("dbo.BulkSMS", new[] { "User_Id" });
            DropIndex("dbo.Audit", new[] { "User_Id" });
            //DropIndex("dbo.Audit", new[] { "ModifiedBy_Id" });
            //DropIndex("dbo.Audit", new[] { "CreatedBy_Id" });
            DropIndex("dbo.aspnet_Roles", new[] { "ApplicationId" });
            DropIndex("dbo.aspnet_Profile", new[] { "UserId" });
            DropIndex("dbo.aspnet_PersonalizationAllUsers", new[] { "PathId" });
            DropIndex("dbo.aspnet_Paths", new[] { "ApplicationId" });
            DropIndex("dbo.aspnet_PersonalizationPerUser", new[] { "UserId" });
            DropIndex("dbo.aspnet_PersonalizationPerUser", new[] { "PathId" });
            DropIndex("dbo.aspnet_Users", new[] { "ApplicationId" });
            DropIndex("dbo.aspnet_Membership", new[] { "ApplicationId" });
            DropIndex("dbo.aspnet_Membership", new[] { "UserId" });
            DropTable("dbo.Product_Category");
            DropTable("dbo.aspnet_UsersInRoles");
            DropTable("dbo.Spoil");
            DropTable("dbo.Category");
            DropTable("dbo.Product");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Customer");
            DropTable("dbo.Order");
            DropTable("dbo.BulkSMS");
            DropTable("dbo.Audit");
            DropTable("dbo.aspnet_Roles");
            DropTable("dbo.aspnet_Profile");
            DropTable("dbo.aspnet_PersonalizationAllUsers");
            DropTable("dbo.aspnet_Paths");
            DropTable("dbo.aspnet_PersonalizationPerUser");
            DropTable("dbo.aspnet_Users");
            DropTable("dbo.aspnet_Membership");
            DropTable("dbo.aspnet_Applications");
        }
    }
}
