using System.Data.Entity.Migrations;

namespace SmartWr.Ipos.Core.Migrations
{
    public partial class aspnet_membership_Procedures : DbMigration
    {
        public override void Up()
        {
            var tsql_schemaChekVersion = @"
                                    IF (EXISTS( SELECT  *
                                                FROM    dbo.aspnet_SchemaVersions
                                                WHERE   Feature = LOWER( @Feature ) AND
                                                        CompatibleSchemaVersion = @CompatibleSchemaVersion ))
                                        RETURN 0

                                    RETURN 1";

            CreateStoredProcedure("[dbo].[aspnet_CheckSchemaVersion]", t => new
            {
                Feature = t.String(storeType: "nvarchar", maxLength: 128),
                CompatibleSchemaVersion = t.String(storeType: "nvarchar", maxLength: 128)
            }, tsql_schemaChekVersion);


            var tsql_createApplication = @" 
                                        SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

                                            IF(@ApplicationId IS NULL)
                                            BEGIN
                                                DECLARE @TranStarted   bit
                                                SET @TranStarted = 0

                                                IF( @@TRANCOUNT = 0 )
                                                BEGIN
	                                                BEGIN TRANSACTION
	                                                SET @TranStarted = 1
                                                END
                                                ELSE
    	                                            SET @TranStarted = 0

                                                SELECT  @ApplicationId = ApplicationId
                                                FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK)
                                                WHERE LOWER(@ApplicationName) = LoweredApplicationName

                                                IF(@ApplicationId IS NULL)
                                                BEGIN
                                                    SELECT  @ApplicationId = NEWID()
                                                    INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
                                                    VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
                                                END


                                                IF( @TranStarted = 1 )
                                                BEGIN
                                                    IF(@@ERROR = 0)
                                                    BEGIN
	                                                SET @TranStarted = 0
	                                                COMMIT TRANSACTION
                                                    END
                                                    ELSE
                                                    BEGIN
                                                        SET @TranStarted = 0
                                                        ROLLBACK TRANSACTION
                                                    END
                                                END
                                            END";


            CreateStoredProcedure("[dbo].[aspnet_Applications_CreateApplication]", t => new
            {
                ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
                ApplicationId = t.Guid(outParameter: true)
            }, tsql_createApplication);


            var tsql_aspnetCreateUser = @"
                                    IF( @UserId IS NULL )
                                        SELECT @UserId = NEWID()
                                    ELSE
                                    BEGIN
                                        IF( EXISTS( SELECT UserId FROM dbo.aspnet_Users
                                                    WHERE @UserId = UserId ) )
                                            RETURN -1
                                    END

                                    INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
                                    VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)

                                    RETURN 0";

            CreateStoredProcedure("[dbo].[aspnet_Users_CreateUser]", t => new
            {
                ApplicationId = t.Guid(),
                UserName = t.String(storeType: "nvarchar", maxLength: 256),
                IsUserAnonymous = t.Boolean(),
                LastActivityDate = t.DateTime(),
                UserId = t.Guid(outParameter: true)
            }, tsql_aspnetCreateUser);


            var tsql_membershipCreateUser = @" DECLARE @ApplicationId uniqueidentifier
                                                    SELECT  @ApplicationId = NULL

                                                    DECLARE @NewUserId uniqueidentifier
                                                    SELECT @NewUserId = NULL

                                                    DECLARE @IsLockedOut bit
                                                    SET @IsLockedOut = 0

                                                    DECLARE @LastLockoutDate  datetime
                                                    SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )

                                                    DECLARE @FailedPasswordAttemptCount int
                                                    SET @FailedPasswordAttemptCount = 0

                                                    DECLARE @FailedPasswordAttemptWindowStart  datetime
                                                    SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

                                                    DECLARE @FailedPasswordAnswerAttemptCount int
                                                    SET @FailedPasswordAnswerAttemptCount = 0

                                                    DECLARE @FailedPasswordAnswerAttemptWindowStart  datetime
                                                    SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

                                                    DECLARE @NewUserCreated bit
                                                    DECLARE @ReturnValue   int
                                                    SET @ReturnValue = 0

                                                    DECLARE @ErrorCode     int
                                                    SET @ErrorCode = 0

                                                    DECLARE @TranStarted   bit
                                                    SET @TranStarted = 0

                                                    IF( @@TRANCOUNT = 0 )
                                                    BEGIN
	                                                    BEGIN TRANSACTION
	                                                    SET @TranStarted = 1
                                                    END
                                                    ELSE
    	                                                SET @TranStarted = 0

                                                    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

                                                    IF( @@ERROR <> 0 )
                                                    BEGIN
                                                        SET @ErrorCode = -1
                                                        GOTO Cleanup
                                                    END

                                                    SET @CreateDate = @CurrentTimeUtc

                                                    SELECT  @NewUserId = UserId FROM dbo.aspnet_Users WHERE LOWER(@UserName) = LoweredUserName AND @ApplicationId = ApplicationId
                                                    IF ( @NewUserId IS NULL )
                                                    BEGIN
                                                        SET @NewUserId = @UserId
                                                        EXEC @ReturnValue = dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CreateDate, @NewUserId OUTPUT
                                                        SET @NewUserCreated = 1
                                                    END
                                                    ELSE
                                                    BEGIN
                                                        SET @NewUserCreated = 0
                                                        IF( @NewUserId <> @UserId AND @UserId IS NOT NULL )
                                                        BEGIN
                                                            SET @ErrorCode = 6
                                                            GOTO Cleanup
                                                        END
                                                    END

                                                    IF( @@ERROR <> 0 )
                                                    BEGIN
                                                        SET @ErrorCode = -1
                                                        GOTO Cleanup
                                                    END

                                                    IF( @ReturnValue = -1 )
                                                    BEGIN
                                                        SET @ErrorCode = 10
                                                        GOTO Cleanup
                                                    END

                                                    IF ( EXISTS ( SELECT UserId
                                                                  FROM   dbo.aspnet_Membership
                                                                  WHERE  @NewUserId = UserId ) )
                                                    BEGIN
                                                        SET @ErrorCode = 6
                                                        GOTO Cleanup
                                                    END

                                                    SET @UserId = @NewUserId

                                                    IF (@UniqueEmail = 1)
                                                    BEGIN
                                                        IF (EXISTS (SELECT *
                                                                    FROM  dbo.aspnet_Membership m WITH ( UPDLOCK, HOLDLOCK )
                                                                    WHERE ApplicationId = @ApplicationId AND LoweredEmail = LOWER(@Email)))
                                                        BEGIN
                                                            SET @ErrorCode = 7
                                                            GOTO Cleanup
                                                        END
                                                    END

                                                    IF (@NewUserCreated = 0)
                                                    BEGIN
                                                        UPDATE dbo.aspnet_Users
                                                        SET    LastActivityDate = @CreateDate
                                                        WHERE  @UserId = UserId
                                                        IF( @@ERROR <> 0 )
                                                        BEGIN
                                                            SET @ErrorCode = -1
                                                            GOTO Cleanup
                                                        END
                                                    END

                                                    INSERT INTO dbo.aspnet_Membership
                                                                ( ApplicationId,
                                                                  UserId,
                                                                  Password,
                                                                  PasswordSalt,
                                                                  Email,
                                                                  LoweredEmail,
                                                                  PasswordQuestion,
                                                                  PasswordAnswer,
                                                                  PasswordFormat,
                                                                  IsApproved,
                                                                  IsLockedOut,
                                                                  CreateDate,
                                                                  LastLoginDate,
                                                                  LastPasswordChangedDate,
                                                                  LastLockoutDate,
                                                                  FailedPasswordAttemptCount,
                                                                  FailedPasswordAttemptWindowStart,
                                                                  FailedPasswordAnswerAttemptCount,
                                                                  FailedPasswordAnswerAttemptWindowStart )
                                                         VALUES ( @ApplicationId,
                                                                  @UserId,
                                                                  @Password,
                                                                  @PasswordSalt,
                                                                  @Email,
                                                                  LOWER(@Email),
                                                                  @PasswordQuestion,
                                                                  @PasswordAnswer,
                                                                  @PasswordFormat,
                                                                  @IsApproved,
                                                                  @IsLockedOut,
                                                                  @CreateDate,
                                                                  @CreateDate,
                                                                  @CreateDate,
                                                                  @LastLockoutDate,
                                                                  @FailedPasswordAttemptCount,
                                                                  @FailedPasswordAttemptWindowStart,
                                                                  @FailedPasswordAnswerAttemptCount,
                                                                  @FailedPasswordAnswerAttemptWindowStart )

                                                    IF( @@ERROR <> 0 )
                                                    BEGIN
                                                        SET @ErrorCode = -1
                                                        GOTO Cleanup
                                                    END

                                                    IF( @TranStarted = 1 )
                                                    BEGIN
	                                                    SET @TranStarted = 0
	                                                    COMMIT TRANSACTION
                                                    END

                                                    RETURN 0

                                                Cleanup:

                                                    IF( @TranStarted = 1 )
                                                    BEGIN
                                                        SET @TranStarted = 0
    	                                                ROLLBACK TRANSACTION
                                                    END

                                                    RETURN @ErrorCode";

            CreateStoredProcedure("[dbo].[aspnet_Membership_CreateUser]", t => new
           {
               ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
               UserName = t.String(storeType: "nvarchar", maxLength: 256),
               Password = t.String(storeType: "nvarchar", maxLength: 128),
               PasswordSalt = t.String(storeType: "nvarchar", maxLength: 128),
               Email = t.String(storeType: "nvarchar", maxLength: 256),
               PasswordQuestion = t.String(storeType: "nvarchar", maxLength: 128),
               PasswordAnswer = t.String(storeType: "nvarchar", maxLength: 128),
               IsApproved = t.Boolean(),
               CurrentTimeUtc = t.DateTime(),
               CreateDate = t.DateTime(defaultValueSql: "NULL"),
               UniqueEmail = t.Int(defaultValueSql: "0"),
               PasswordFormat = t.Int(defaultValueSql: "0"),
               UserId = t.Guid(outParameter: true)
           }, tsql_membershipCreateUser);



            var tsql_membershipGetUserByName = @"
                                                    DECLARE @UserId uniqueidentifier

                                                    IF (@UpdateLastActivity = 1)
                                                    BEGIN
                                                        -- select user ID from aspnet_users table
                                                        SELECT TOP 1 @UserId = u.UserId
                                                        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
                                                        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                                                                u.ApplicationId = a.ApplicationId    AND
                                                                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

                                                        IF (@@ROWCOUNT = 0) -- Username not found
                                                            RETURN -1

                                                        UPDATE   dbo.aspnet_Users
                                                        SET      LastActivityDate = @CurrentTimeUtc
                                                        WHERE    @UserId = UserId

                                                        SELECT m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                                                                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                                                                u.UserId, m.IsLockedOut, m.LastLockoutDate
                                                        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
                                                        WHERE  @UserId = u.UserId AND u.UserId = m.UserId 
                                                    END
                                                    ELSE
                                                    BEGIN
                                                        SELECT TOP 1 m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                                                                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                                                                u.UserId, m.IsLockedOut,m.LastLockoutDate
                                                        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
                                                        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                                                                u.ApplicationId = a.ApplicationId    AND
                                                                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

                                                        IF (@@ROWCOUNT = 0) -- Username not found
                                                            RETURN -1
                                                    END

                                                    RETURN 0
                                                ";

            CreateStoredProcedure("[dbo].[aspnet_Membership_GetUserByName]", t => new
            {
                ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
                UserName = t.String(storeType: "nvarchar", maxLength: 256),
                CurrentTimeUtc = t.DateTime(),
                UpdateLastActivity = t.Boolean(defaultValueSql: "0")
            }, tsql_membershipGetUserByName);

            var tsql_getAllUsers = @"
                                                    DECLARE @ApplicationId uniqueidentifier
                                                    SELECT  @ApplicationId = NULL
                                                    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
                                                    IF (@ApplicationId IS NULL)
                                                        RETURN 0


                                                    -- Set the page bounds
                                                    DECLARE @PageLowerBound int
                                                    DECLARE @PageUpperBound int
                                                    DECLARE @TotalRecords   int
                                                    SET @PageLowerBound = @PageSize * @PageIndex
                                                    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

                                                    -- Create a temp table TO store the select results
                                                    CREATE TABLE #PageIndexForUsers
                                                    (
                                                        IndexId int IDENTITY (0, 1) NOT NULL,
                                                        UserId uniqueidentifier
                                                    )

                                                    -- Insert into our temp table
                                                    INSERT INTO #PageIndexForUsers (UserId)
                                                    SELECT u.UserId
                                                    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u
                                                    WHERE  u.ApplicationId = @ApplicationId AND u.UserId = m.UserId
                                                    ORDER BY u.UserName

                                                    SELECT @TotalRecords = @@ROWCOUNT

                                                    SELECT u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                                                            m.CreateDate,
                                                            m.LastLoginDate,
                                                            u.LastActivityDate,
                                                            m.LastPasswordChangedDate,
                                                            u.UserId, m.IsLockedOut,
                                                            m.LastLockoutDate
                                                    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
                                                    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
                                                           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
                                                    ORDER BY u.UserName
                                                    RETURN @TotalRecords";
            CreateStoredProcedure("[dbo].[aspnet_Membership_GetAllUsers]", t => new
            {

                ApplicationName = t.String(256),
                PageIndex = t.Int(),
                PageSize = t.Int()

            }, tsql_getAllUsers);


            string tsql_membershipSetPassword = @"

                                  DECLARE @UserId uniqueidentifier
                                    SELECT  @UserId = NULL
                                    SELECT  @UserId = u.UserId
                                    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
                                    WHERE   LoweredUserName = LOWER(@UserName) AND
                                            u.ApplicationId = a.ApplicationId  AND
                                            LOWER(@ApplicationName) = a.LoweredApplicationName AND
                                            u.UserId = m.UserId

                                    IF (@UserId IS NULL)
                                        RETURN(1)

                                    UPDATE dbo.aspnet_Membership
                                    SET Password = @NewPassword, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt,
                                        LastPasswordChangedDate = @CurrentTimeUtc
                                    WHERE @UserId = UserId
                                    RETURN(0)";


            CreateStoredProcedure("[dbo].[aspnet_Membership_SetPassword]", t => new
            {
                ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
                UserName = t.String(storeType: "nvarchar", maxLength: 256),
                NewPassword = t.String(storeType: "nvarchar", maxLength: 128),
                PasswordSalt = t.String(storeType: "nvarchar", maxLength: 128),
                CurrentTimeUtc = t.DateTime(),
                PasswordFormat = t.Int(defaultValueSql: "0"),
            }, tsql_membershipSetPassword);


            var tsql_aspnetDeleteUser = @"

                                DECLARE @UserId               uniqueidentifier
                                    SELECT  @UserId               = NULL
                                    SELECT  @NumTablesDeletedFrom = 0

                                    DECLARE @TranStarted   bit
                                    SET @TranStarted = 0

                                    IF( @@TRANCOUNT = 0 )
                                    BEGIN
	                                    BEGIN TRANSACTION
	                                    SET @TranStarted = 1
                                    END
                                    ELSE
	                                SET @TranStarted = 0

                                    DECLARE @ErrorCode   int
                                    DECLARE @RowCount    int

                                    SET @ErrorCode = 0
                                    SET @RowCount  = 0

                                    SELECT  @UserId = u.UserId
                                    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a
                                    WHERE   u.LoweredUserName       = LOWER(@UserName)
                                        AND u.ApplicationId         = a.ApplicationId
                                        AND LOWER(@ApplicationName) = a.LoweredApplicationName

                                    IF (@UserId IS NULL)
                                    BEGIN
                                        GOTO Cleanup
                                    END

                                    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
                                    IF ((@TablesToDeleteFrom & 1) <> 0 AND
                                        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
                                    BEGIN
                                        DELETE FROM dbo.aspnet_Membership WHERE @UserId = UserId

                                        SELECT @ErrorCode = @@ERROR,
                                               @RowCount = @@ROWCOUNT

                                        IF( @ErrorCode <> 0 )
                                            GOTO Cleanup

                                        IF (@RowCount <> 0)
                                            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
                                    END

                                    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
                                    IF ((@TablesToDeleteFrom & 2) <> 0  AND
                                        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_UsersInRoles') AND (type = 'V'))) )
                                    BEGIN
                                        DELETE FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId

                                        SELECT @ErrorCode = @@ERROR,
                                                @RowCount = @@ROWCOUNT

                                        IF( @ErrorCode <> 0 )
                                            GOTO Cleanup

                                        IF (@RowCount <> 0)
                                            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
                                    END

                                    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
                                    IF ((@TablesToDeleteFrom & 4) <> 0  AND
                                        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
                                    BEGIN
                                        DELETE FROM dbo.aspnet_Profile WHERE @UserId = UserId

                                        SELECT @ErrorCode = @@ERROR,
                                                @RowCount = @@ROWCOUNT

                                        IF( @ErrorCode <> 0 )
                                            GOTO Cleanup

                                        IF (@RowCount <> 0)
                                            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
                                    END

                                    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
                                    IF ((@TablesToDeleteFrom & 8) <> 0  AND
                                        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
                                    BEGIN
                                        DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE @UserId = UserId

                                        SELECT @ErrorCode = @@ERROR,
                                                @RowCount = @@ROWCOUNT

                                        IF( @ErrorCode <> 0 )
                                            GOTO Cleanup

                                        IF (@RowCount <> 0)
                                            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
                                    END

                                    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
                                    IF ((@TablesToDeleteFrom & 1) <> 0 AND
                                        (@TablesToDeleteFrom & 2) <> 0 AND
                                        (@TablesToDeleteFrom & 4) <> 0 AND
                                        (@TablesToDeleteFrom & 8) <> 0 AND
                                        (EXISTS (SELECT UserId FROM dbo.aspnet_Users WHERE @UserId = UserId)))
                                    BEGIN
                                        DELETE FROM dbo.aspnet_Users WHERE @UserId = UserId

                                        SELECT @ErrorCode = @@ERROR,
                                                @RowCount = @@ROWCOUNT

                                        IF( @ErrorCode <> 0 )
                                            GOTO Cleanup

                                        IF (@RowCount <> 0)
                                            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
                                    END

                                    IF( @TranStarted = 1 )
                                    BEGIN
	                                    SET @TranStarted = 0
	                                    COMMIT TRANSACTION
                                    END

                                    RETURN 0

                                Cleanup:
                                    SET @NumTablesDeletedFrom = 0

                                    IF( @TranStarted = 1 )
                                    BEGIN
                                        SET @TranStarted = 0
	                                    ROLLBACK TRANSACTION
                                    END

                                    RETURN @ErrorCode

";
            CreateStoredProcedure("[dbo].[aspnet_Users_DeleteUser]", t => new
            {
                ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
                UserName = t.String(storeType: "nvarchar", maxLength: 256),
                TablesToDeleteFrom = t.Int(),
                NumTablesDeletedFrom = t.Int(outParameter: true)
            }, tsql_aspnetDeleteUser);


            var tsql_aspnet_Membership_GetPasswordWithFormat = @"
                    DECLARE @IsLockedOut                        bit
                        DECLARE @UserId                             uniqueidentifier
                        DECLARE @Password                           nvarchar(128)
                        DECLARE @PasswordSalt                       nvarchar(128)
                        DECLARE @PasswordFormat                     int
                        DECLARE @FailedPasswordAttemptCount         int
                        DECLARE @FailedPasswordAnswerAttemptCount   int
                        DECLARE @IsApproved                         bit
                        DECLARE @LastActivityDate                   datetime
                        DECLARE @LastLoginDate                      datetime

                        SELECT  @UserId          = NULL

                        SELECT  @UserId = u.UserId, @IsLockedOut = m.IsLockedOut, @Password=Password, @PasswordFormat=PasswordFormat,
                                @PasswordSalt=PasswordSalt, @FailedPasswordAttemptCount=FailedPasswordAttemptCount,
		                        @FailedPasswordAnswerAttemptCount=FailedPasswordAnswerAttemptCount, @IsApproved=IsApproved,
                                @LastActivityDate = LastActivityDate, @LastLoginDate = LastLoginDate
                        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
                        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                                u.ApplicationId = a.ApplicationId    AND
                                u.UserId = m.UserId AND
                                LOWER(@UserName) = u.LoweredUserName

                        IF (@UserId IS NULL)
                            RETURN 1

                        IF (@IsLockedOut = 1)
                            RETURN 99

                        SELECT   @Password, @PasswordFormat, @PasswordSalt, @FailedPasswordAttemptCount,
                                 @FailedPasswordAnswerAttemptCount, @IsApproved, @LastLoginDate, @LastActivityDate

                        IF (@UpdateLastLoginActivityDate = 1 AND @IsApproved = 1)
                        BEGIN
                            UPDATE  dbo.aspnet_Membership
                            SET     LastLoginDate = @CurrentTimeUtc
                            WHERE   UserId = @UserId

                            UPDATE  dbo.aspnet_Users
                            SET     LastActivityDate = @CurrentTimeUtc
                            WHERE   @UserId = UserId
                        END


                        RETURN 0";


            CreateStoredProcedure("[dbo].[aspnet_Membership_GetPasswordWithFormat]", t => new {

                ApplicationName = t.String(storeType: "nvarchar", maxLength: 256),
                UserName = t.String(storeType: "nvarchar", maxLength: 256),
                UpdateLastLoginActivityDate = t.Boolean(),
                CurrentTimeUtc =t.DateTime(),
            }, tsql_aspnet_Membership_GetPasswordWithFormat);
        }

        public override void Down()
        {
            DropStoredProcedure("[dbo].[aspnet_CheckSchemaVersion]");
            DropStoredProcedure("[dbo].[aspnet_Applications_CreateApplication]");
            DropStoredProcedure("[dbo].[aspnet_Users_CreateUser]");
            DropStoredProcedure("[dbo].[aspnet_Membership_CreateUser]");
            DropStoredProcedure("[dbo].[aspnet_Membership_GetUserByName]");
            DropStoredProcedure("[dbo].[aspnet_Membership_GetAllUsers]");
            DropStoredProcedure("[dbo].[aspnet_Membership_SetPassword]");
            DropStoredProcedure("[dbo].[aspnet_Users_DeleteUser]");
            DropStoredProcedure("[dbo].[aspnet_Membership_GetPasswordWithFormat]");
        }
    }
}