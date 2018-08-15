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
                  AND NAME = 'Sp_RetrieveSpoilsBy')
  DROP PROCEDURE [sync].[Sp_RetrieveSpoilsBy]

GO

CREATE PROCEDURE [sync].[Sp_retrievespoilsby] @Id UNIQUEIDENTIFIER = NULL
AS
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
           JOIN dbo.aspnet_Users cashier
             ON cashier.UserId = s.User_Id
           JOIN Product p
             ON p.ProductId = s.Product_Id
    WHERE  s.SpoilId = @Id 
