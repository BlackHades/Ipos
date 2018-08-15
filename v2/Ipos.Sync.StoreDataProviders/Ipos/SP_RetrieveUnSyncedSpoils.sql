USE [Iposv3]

IF NOT EXISTS (SELECT schema_name
               FROM   information_schema.schemata
               WHERE  schema_name = 'sync')
  BEGIN
      EXEC Sp_executesql
        N'CREATE SCHEMA sync'
  END

GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  type = 'P'
                  AND NAME = 'Sp_RetrieveUnsyncedSpoils')
  DROP PROCEDURE [sync].[Sp_RetrieveUnsyncedSpoils]

GO

CREATE PROCEDURE [sync].[Sp_retrieveunsyncedspoils] @EntryDate VARCHAR(25) = NULL
AS
    SET DATEFORMAT ymd;

    DECLARE @enDate DATETIME

    IF Isdate(@EntryDate) <> 1
      SET @enDate = CONVERT(DATETIME, Getdate())
    ELSE
      SET @enDate = CONVERT(DATETIME, @EntryDate, 121)

    SET DATEFORMAT dmy;

    SELECT Cast (s.SpoilId AS VARCHAR(50)) TransactionRefNo,
           s.Product_Id                    StockRefNo,
           s.[Description]                 SpoilDetails,
           s.Title                         StockDetails,
           cashier.UserName                ReportedBy,
           s.Quantity                      StockUnit,
           s.IsDeleted,
           s.EntryDate                     RefCreatedDate,
           s.ModifiedOnUtc                 RefModifiedDate,
           p.CostPrice                     Cost
    FROM   dbo.Spoil s
           LEFT JOIN dbo.aspnet_Users cashier
                  ON cashier.UserId = s.User_Id
           JOIN Product p
             ON p.ProductId = s.Product_Id
    WHERE  ( ( @EntryDate IS NULL )
              OR s.EntryDate > @enDate )
    ORDER  BY s.ModifiedOnUtc ASC,
              s.EntryDate ASC 
