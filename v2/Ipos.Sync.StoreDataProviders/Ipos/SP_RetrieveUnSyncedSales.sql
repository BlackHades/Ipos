USE [Iposv3]

IF NOT EXISTS (
SELECT  schema_name
FROM    information_schema.schemata
WHERE   schema_name = 'sync' ) 

BEGIN
EXEC sp_executesql N'CREATE SCHEMA sync'
END

GO

CREATE PROCEDURE [sync].[SP_RetrieveUnSyncedSales]
	@EntryDate VARCHAR(25) = NULL,
	@ROWLIMIT INT  = 2
AS

	SET ROWCOUNT @ROWLIMIT
	SET DATEFORMAT ymd;

	DECLARE @enDate DATETIME

	IF ISDATE(@EntryDate) <> 1
	Set @enDate = Convert(DATETIME, GETDATE())
	Else
	Set @enDate = CONVERT(DATETIME, @EntryDate, 121)

	SET DATEFORMAT dmy;
	SELECT CAST(ordt.Order_UId AS VARCHAR(50)) TransactionRefNo, -- 0
	CAST(ordt.OrderDetailUId AS VARCHAR(50)) StockRefNo, -- 1
	CAST(ISNULL(ordt.Product_Id, '') AS VARCHAR(15)) StockNo, -- 2
	ISNULL(prod.Name +' - ' + prod.[Description], '') StockDetails,  --3
	CAST(ctg.CategoryUId AS VARCHAR(50)) StockCategoryRefNo, -- 4
	ISNULL(ctg.Name + ' - ' + ctg.[Description], '') StockCategoryDetails, --5
	ISNULL(ordt.Quantiy, 0) StockUnitPurchased, --6
	ISNULL(ordt.Price, 0.0) StockUnitAmount, --7
	ISNULL(ordt.CostPrice, 0.0) StockCostAmount, --8
	ISNULL(ordt.Discount, 0.0)  StockDiscountAmount, --9
	ordt.EntryDate RefCreatedDate, -- 10
	cst.LastName + ' ' + cst.FirstName  CustomerName, -- 11
	cst.Email CustomerEmail, --12
	cst.[Address] CustomerAddress,--13
	cst.PhoneNo CustomerTel,--14
	cst.Sex CustomerGender, -- 15
	prod.Barcode Barcode,--16
	ordt.ModifiedOnUtc RefModifiedDate, --17
	prod.quantity StockUnitLeft, --18
	Seller.UserName Cashier, --19,
	prod.ReorderLevel StockReorderUnit, --20
	ord.Remark --21
	FROM [dbo].[OrderDetails] ordt
	LEFT JOIN [dbo].[Order] ord
	ON ordt.Order_UId = ord.OrderUId
	LEFT JOIN Product prod
	ON ordt.Product_Id = prod.ProductId
	LEFT JOIN Customer cst
	ON ord.Customer_UId = cst.CustomerId
	LEFT JOIN Category ctg
	ON prod.Category_UId = ctg.CategoryUId
	LEFT JOIN aspnet_Users seller
	ON Seller.UserId= ord.User_Id
	WHERE ((@EntryDate IS NULL) OR ordt.EntryDate > @enDate)
	ORDER BY ordt.EntryDate ASC