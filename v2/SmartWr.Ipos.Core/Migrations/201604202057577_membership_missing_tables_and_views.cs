using SmartWr.Ipos.Core.Settings;
using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class membership_missing_tables : DbMigration
    {
        public override void Up()
        {
            if (!IposConfig.UseMembership)
                return;

            //Tables
            CreateTable(
                "dbo.aspnet_SchemaVersions",
                c => new
                    {
                        Feature = c.String(nullable: false, maxLength: 128),
                        CompatibleSchemaVersion = c.String(nullable: false, maxLength: 128),
                        IsCurrentVersion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.Feature, t.CompatibleSchemaVersion });

            CreateTable(
                "dbo.aspnet_WebEvent_Events",
                c => new
                    {
                        EventId = c.String(nullable: false, maxLength: 32, fixedLength: true),
                        EventTimeUtc = c.DateTime(nullable: false),
                        EventTime = c.DateTime(nullable: false),
                        EventType = c.String(nullable: false, maxLength: 256),
                        EventSequence = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EventOccurrence = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EventCode = c.Int(nullable: false),
                        EventDetailCode = c.Int(nullable: false),
                        Message = c.String(maxLength: 1024),
                        ApplicationPath = c.String(maxLength: 256),
                        ApplicationVirtualPath = c.String(maxLength: 256),
                        MachineName = c.String(nullable: false, maxLength: 256),
                        RequestUrl = c.String(maxLength: 1024),
                        ExceptionType = c.String(maxLength: 256),
                        Details = c.String(),
                    })
                .PrimaryKey(t => t.EventId);

            //Views
            var tsqlvw_aspnet_MembershipUsers = @"
               CREATE VIEW[dbo].[vw_aspnet_MembershipUsers]
               AS SELECT[dbo].[aspnet_Membership].[UserId], [dbo].[aspnet_Membership].[PasswordFormat], [dbo].[aspnet_Membership].[MobilePIN], [dbo].[aspnet_Membership].[Email], [dbo].[aspnet_Membership].[LoweredEmail], [dbo].[aspnet_Membership].[PasswordQuestion], [dbo].[aspnet_Membership].[PasswordAnswer], [dbo].[aspnet_Membership].[IsApproved], [dbo].[aspnet_Membership].[IsLockedOut], [dbo].[aspnet_Membership].[CreateDate], [dbo].[aspnet_Membership].[LastLoginDate], [dbo].[aspnet_Membership].[LastPasswordChangedDate], [dbo].[aspnet_Membership].[LastLockoutDate], [dbo].[aspnet_Membership].[FailedPasswordAttemptCount], [dbo].[aspnet_Membership].[FailedPasswordAttemptWindowStart], [dbo].[aspnet_Membership].[FailedPasswordAnswerAttemptCount], [dbo].[aspnet_Membership].[FailedPasswordAnswerAttemptWindowStart], [dbo].[aspnet_Membership].[Comment], [dbo].[aspnet_Users].[ApplicationId], [dbo].[aspnet_Users].[UserName], [dbo].[aspnet_Users].[MobileAlias], [dbo].[aspnet_Users].[IsAnonymous], [dbo].[aspnet_Users].[LastActivityDate]
               FROM[dbo].[aspnet_Membership] INNER JOIN[dbo].[aspnet_Users]
               ON[dbo].[aspnet_Membership].[UserId] = [dbo].[aspnet_Users].[UserId] ";

            var tsqlvw_aspnet_UsersInRoles = @"
               CREATE VIEW[dbo].[vw_aspnet_UsersInRoles]
               AS SELECT[dbo].[aspnet_UsersInRoles].[UserId], [dbo].[aspnet_UsersInRoles].[RoleId]
               FROM[dbo].[aspnet_UsersInRoles] ";

            var tsqlvw_aspnet_Profiles = @"
               CREATE VIEW[dbo].[vw_aspnet_Profiles]
               AS SELECT[dbo].[aspnet_Profile].[UserId], [dbo].[aspnet_Profile].[LastUpdatedDate], [DataSize] = DATALENGTH([dbo].[aspnet_Profile].[PropertyNames]) + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesString]) + DATALENGTH([dbo].[aspnet_Profile].[PropertyValuesBinary])
               FROM[dbo].[aspnet_Profile] ";

            var tsqlvw_aspnet_WebPartState_User = @"
               CREATE VIEW[dbo].[vw_aspnet_WebPartState_User]
               AS SELECT[dbo].[aspnet_PersonalizationPerUser].[PathId], [dbo].[aspnet_PersonalizationPerUser].[UserId], [DataSize] = DATALENGTH([dbo].[aspnet_PersonalizationPerUser].[PageSettings]), [dbo].[aspnet_PersonalizationPerUser].[LastUpdatedDate]
               FROM[dbo].[aspnet_PersonalizationPerUser] ";
            Sql(tsqlvw_aspnet_MembershipUsers);
            Sql(tsqlvw_aspnet_UsersInRoles);
            Sql(tsqlvw_aspnet_Profiles);
            Sql(tsqlvw_aspnet_WebPartState_User);
        }

        public override void Down()
        {
            Sql("DROP VIEW [dbo].[vw_aspnet_MembershipUsers]");
            Sql("DROP VIEW [dbo].[vw_aspnet_UsersInRoles]");
            Sql("DROP VIEW [dbo].[vw_aspnet_Profiles]");
            Sql("DROP VIEW [dbo].[vw_aspnet_WebPartState_User]");


            DropTable("dbo.aspnet_WebEvent_Events");
            DropTable("dbo.aspnet_SchemaVersions");
        }
    }
}
