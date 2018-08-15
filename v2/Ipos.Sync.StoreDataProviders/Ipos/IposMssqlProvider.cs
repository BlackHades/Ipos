using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Db;
using Ipos.Sync.StoreDataProviders.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Ipos
{
    public class IposMssqlProvider : DBHelper, IStoreDataProvider, ISyncStoreDataProvider
    {
        public IposMssqlProvider(string connectionString, string providerName)
            : base(connectionString, providerName)
        {
        }

        public void UpdateSyncTransaction(Guid Id, string barcode, string transactionRefNo, string cashier, string customerAddress, DateTime? customerDOB, string customerEmail, string customerName, int? customerGender, string customerTel, DateTime? refCreatedDate, DateTime? refModifiedDate, string stockCategoryLine, string stockCategoryRefNo, decimal stockCostAmount, string stockDetails, double stockDiscountAmount, string stockItemCode, string stockItemNo, string stockRefNo, decimal stockUnitAmount, int? stockUnitLeft, int stockUnitPurchased, decimal taxAmount, bool isDeleted, int stockReorderUnit, bool isSyncReady, int syncStatus)
        {
            String tsql = @"[Sync].[Sp_UpdateSyncTransactionByField]";

            try
            {
                var colParameters = new DBHelper.Parameters[]
                {
                   new DBHelper.Parameters("@Id", Id),
                    new DBHelper.Parameters("@TransactionRefNo", transactionRefNo),
                    new DBHelper.Parameters("@Cashier", cashier),
                    new DBHelper.Parameters("@SyncStatus", syncStatus),
                    new DBHelper.Parameters("@IsSyncReady", isSyncReady),
                    new DBHelper.Parameters("@StockItemNo", stockItemNo),
                    new DBHelper.Parameters("@StockRefNo", stockRefNo),
                    new DBHelper.Parameters("@StockItemCode", stockItemCode),
                    new DBHelper.Parameters("@StockDetails", stockDetails),
                    new DBHelper.Parameters("@StockCategoryRefNo", stockCategoryRefNo),
                    new DBHelper.Parameters("@StockCategoryLine", stockCategoryLine),
                    new DBHelper.Parameters("@StockUnitAmount", stockUnitAmount),
                    new DBHelper.Parameters("@StockUnitLeft", stockUnitLeft),
                    new DBHelper.Parameters("@StockUnitPurchased", stockUnitPurchased),
                    new DBHelper.Parameters("@StockCostAmount", stockCostAmount),
                    new DBHelper.Parameters("@StockDiscountAmount", stockDiscountAmount),
                    new DBHelper.Parameters("@RefCreatedDate", refCreatedDate),
                    new DBHelper.Parameters("@RefModifiedDate", refModifiedDate),
                    new DBHelper.Parameters("@CustomerName", customerName),
                    new DBHelper.Parameters("@CustomerEmail", customerEmail),
                    new DBHelper.Parameters("@CustomerAddress", customerAddress),
                    new DBHelper.Parameters("@CustomerTel", customerTel),
                    new DBHelper.Parameters("@CustomerGender", customerGender),
                    new DBHelper.Parameters("@CustomerDOB", customerDOB),
                    new DBHelper.Parameters("@Barcode", barcode),
                    new DBHelper.Parameters("@TaxAmount", taxAmount),
                    new DBHelper.Parameters("@IsDeleted", isDeleted),
                    new DBHelper.Parameters("@StockReorderUnit", stockReorderUnit),
                };

                this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);
            }
            catch (DBConcurrencyException)
            {
            }
        }


        public void AddTransactionToSync(Guid Id, string reasonSyncFailed, string transactionRefNo, string currencyCode, string cashier, string machineName, DateTime? syncDate, int syncStatus, bool isSyncReady, string stockItemNo, string stockRefNo, string stockItemCode, string stockDetails, string stockCategoryRefNo, string stockCategoryLine, int? stockUnitLeft, int stockUnitPurchased, decimal stockUnitAmount, decimal stockCostAmount, double stockDiscountAmount, DateTime? refModifiedDate, DateTime? refCreatedDate, string customerName, string customerEmail, string customerAddress, string customerTel, int? customerGender, DateTime? customerDOB, string barcode, decimal taxAmount, DateTime createdOnUtc, DateTime? modifiedOnUtc, bool isDeleted, int syncFailedCount, string syncRefNo, DateTime? syncRefCreatedOn, DateTime? syncRefModifiedOn, int stockReorderUnit)
        {
            String tsql = @"[Sync].[Sp_AddTransactionToSync]";

            var colParameters = new DBHelper.Parameters[]

            {
                    new DBHelper.Parameters("@Id", Id),
                new DBHelper.Parameters("@ReasonSyncFailed", reasonSyncFailed),
                new DBHelper.Parameters("@TransactionRefNo", transactionRefNo),
                new DBHelper.Parameters("@CurrencyCode", currencyCode),
                new DBHelper.Parameters("@Cashier", cashier),
                new DBHelper.Parameters("@MachineName", machineName),
                new DBHelper.Parameters("@SyncDate", syncDate),
                new DBHelper.Parameters("@SyncStatus", syncStatus),
                new DBHelper.Parameters("@IsSyncReady", isSyncReady),
                new DBHelper.Parameters("@StockItemNo", stockItemNo),
                new DBHelper.Parameters("@StockRefNo", stockRefNo),
                new DBHelper.Parameters("@StockDetails", stockDetails),
                new DBHelper.Parameters("@StockCategoryRefNo", stockCategoryRefNo),
                new DBHelper.Parameters("@StockCategoryLine", stockCategoryLine),
                new DBHelper.Parameters("@StockUnitPurchased", stockUnitPurchased),
                new DBHelper.Parameters("@StockUnitAmount", stockUnitAmount),
                new DBHelper.Parameters("@StockCostAmount", stockCostAmount),
                new DBHelper.Parameters("@StockDiscountAmount", stockDiscountAmount),
                new DBHelper.Parameters("@RefModifiedDate", refModifiedDate),
                new DBHelper.Parameters("@RefCreatedDate", refCreatedDate),
                new DBHelper.Parameters("@CustomerName", customerName),
                new DBHelper.Parameters("@CustomerAddress", customerAddress),
                new DBHelper.Parameters("@CustomerTel", customerTel),
                new DBHelper.Parameters("@CustomerGender", customerGender),
                new DBHelper.Parameters("@CustomerDOB", customerDOB),
                new DBHelper.Parameters("@Barcode", barcode),
                new DBHelper.Parameters("@TaxAmount", taxAmount),
                new DBHelper.Parameters("@CreatedOnUtc", createdOnUtc),
                new DBHelper.Parameters("@ModifiedOnUtc", modifiedOnUtc),
                new DBHelper.Parameters("@IsDeleted", isDeleted),
                new DBHelper.Parameters("@SyncFailedCount", syncFailedCount),
                new DBHelper.Parameters("@SyncRefNo", syncRefNo),
                new DBHelper.Parameters("@SyncRefCreatedOn", syncRefCreatedOn),
                new DBHelper.Parameters("@SyncRefModifiedOn", syncRefModifiedOn),
                new DBHelper.Parameters("@StockReorderUnit", stockReorderUnit)
            };

            this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);
        }

        public DbDataReader GetLastTransaction()
        {
            String tsql = @"[Sync].[Sp_GetLastTransaction]";
            return this.ExecuteReader(CommandType.StoredProcedure, tsql);
        }

        public DbDataReader GetTransactionBy(string transactionRefNo)
        {
            String tsql = @"SELECT TOP 1 txn.* FROM [Sync].[Transactions] txn
                            WHERE txn.TransactionRefNo = @p0";

            // Structure Parameter Array
            var colParameters = new DBHelper.Parameters[]
            {
                new DBHelper.Parameters("@p0", transactionRefNo),
            };

            return this.ExecuteReader(CommandType.Text, tsql, colParameters);
        }

        public void AddSpoilToSync(Guid Id, string transactionRefNo, int stockRefNo, string stockDetails, int stockUnit, int? stockUnitLeft, bool isSyncReady, int syncStatus, string spoilDetails, string reasonSyncFailed, string reportedBy, DateTime? syncRefCreatedOn, DateTime? syncRefModifiedOn, string machineName, string syncRefNo, DateTime? syncDate, int syncFailedCount, DateTime? refCreatedDate, DateTime? refModifiedDate, double? cost, DateTime createdOnUtc, DateTime? modifiedOnUtc, bool isDeleted)
        {
            String tsql = @"[Sync].[Sp_AddSpoilToSync]";

            var colParameters = new DBHelper.Parameters[]

            {
                    new DBHelper.Parameters("@Id", Id),
                new DBHelper.Parameters("@TransactionRefNo", transactionRefNo),
                new DBHelper.Parameters("@StockRefNo", stockRefNo),
                new DBHelper.Parameters("@StockDetails", stockDetails),
                new DBHelper.Parameters("@StockUnit", stockUnit),
                new DBHelper.Parameters("@StockUnitLeft", stockUnitLeft),
                new DBHelper.Parameters("@IsSyncReady", isSyncReady),
                new DBHelper.Parameters("@SyncStatus", syncStatus),
                new DBHelper.Parameters("@SpoilDetails", spoilDetails),
                new DBHelper.Parameters("@ReasonSyncFailed", reasonSyncFailed),
                new DBHelper.Parameters("@ReportedBy", reportedBy),
                    new DBHelper.Parameters("@SyncRefCreatedOn", syncRefCreatedOn),
                new DBHelper.Parameters("@SyncRefModifiedOn", syncRefModifiedOn),
                new DBHelper.Parameters("@MachineName", machineName),
                new DBHelper.Parameters("@SyncRefNo", syncRefNo),
                new DBHelper.Parameters("@SyncDate", syncDate),
                new DBHelper.Parameters("@SyncFailedCount", syncFailedCount),
                new DBHelper.Parameters("@RefCreatedDate", refCreatedDate),
                new DBHelper.Parameters("@RefModifiedDate", refModifiedDate),
                new DBHelper.Parameters("@Cost", cost),
                new DBHelper.Parameters("@CreatedOnUtc", createdOnUtc),
                new DBHelper.Parameters("@ModifiedOnUtc", modifiedOnUtc),
                new DBHelper.Parameters("@IsDeleted", isDeleted),
            };

            this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);

        }
        public void UpdateSyncedSpoil(Int32 StockRefNo, String StockDetails,
            Int32 StockUnit, Int32? StockUnitLeft,
            bool IsSyncReady, Int32 SyncStatus,
            String SpoilDetails, String ReportedBy, String MachineName,
            DateTime? RefCreatedDate, DateTime? RefModifiedDate,
            Double? Cost, Guid Id)
        {
            String tsql = @"[Sync].[Sp_UpdateSyncSpoilByField]";

            try
            {
                var colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@StockRefNo", StockRefNo),
                    new DBHelper.Parameters("@StockDetails", StockDetails),
                    new DBHelper.Parameters("@StockUnit", StockUnit),
                    new DBHelper.Parameters("@StockUnitLeft", StockUnitLeft),
                    new DBHelper.Parameters("@IsSyncReady", IsSyncReady),
                    new DBHelper.Parameters("@SyncStatus", SyncStatus),
                    new DBHelper.Parameters("@SpoilDetails", SpoilDetails),
                    new DBHelper.Parameters("@ReportedBy", ReportedBy),
                    new DBHelper.Parameters("@MachineName", MachineName),
                    new DBHelper.Parameters("@RefCreatedDate", RefCreatedDate),
                    new DBHelper.Parameters("@RefModifiedDate", RefModifiedDate),
                     new DBHelper.Parameters("@Cost", Cost),
                    new DBHelper.Parameters("@Id", Id)
                };

                this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);
            }
            catch (DBConcurrencyException)
            {
            }
        }

        public DbDataReader GetSpoiltBy(string transactionRefNo)
        {
            String tsql = @"SELECT TOP 1 spl.* FROM [Sync].[Spoils] spl
                            WHERE spl.TransactionRefNo = @p0";

            // Structure Parameter Array
            var colParameters = new DBHelper.Parameters[]
            {
                new DBHelper.Parameters("@p0", transactionRefNo),
            };

            return this.ExecuteReader(CommandType.Text, tsql, colParameters);
        }

        public void UpdateSyncedSpoil(SpoilDto txn)
        {
            String tsql = @"[Sync].[Sp_UpdateSyncSpoil]";

            try
            {
                var colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@IsSyncReady", txn.IsSyncReady),
                    new DBHelper.Parameters("@SyncDate", txn.SyncDate),
                    new DBHelper.Parameters("@SyncRefNo", txn.SyncRefNo?? String.Empty),
                    new DBHelper.Parameters("@SyncRefCreatedOn", txn.SyncRefCreatedOn),
                    new DBHelper.Parameters("@SyncRefModifiedOn", txn.SyncRefModifiedOn??txn.SyncRefCreatedOn),
                    new DBHelper.Parameters("@ReasonSyncFailed", txn.ReasonSyncFailed ?? string.Empty),
                     new DBHelper.Parameters("@SyncFailedCount", txn.SyncFailedCount),
                    new DBHelper.Parameters("@Id", txn.Id)
                };

                this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);
            }
            catch (DBConcurrencyException)
            {
            }
        }

        public DbDataReader GetLastSpoil()
        {
            String tsql = @"[Sync].[Sp_GetLastSpoil]";
            return this.ExecuteReader(CommandType.StoredProcedure, tsql);
        }

        public List<SpoilDto> GetRangeUnsyncedSpoil(String count, int status, bool syncReady)
        {
            String tsql = $@"SELECT TOP {count} spl.* FROM [Sync].[Spoils] spl
                            WHERE spl.SyncStatus = @p0 AND spl.IsSyncReady = @p1";

            List<SpoilDto> txnSpoilList = new List<SpoilDto>();

            try
            {
                // Structure Parameter Array
                var colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@p0", status),
                    new DBHelper.Parameters("@p1", syncReady)
                };

                using (IDataReader drSalesTrans = this.ExecuteReader(CommandType.Text, tsql, colParameters))
                {
                    while (drSalesTrans.Read())
                    {
                        var spoiltDto = new SpoilDto();

                        spoiltDto.Id = drSalesTrans.GetGuid(0);
                        spoiltDto.TransactionRefNo = drSalesTrans.GetString(1);
                        spoiltDto.StockRefNo = drSalesTrans.GetInt32(2);
                        spoiltDto.StockDetails = drSalesTrans.IsDBNull(3) ? null : drSalesTrans.GetString(3);
                        spoiltDto.StockUnit = drSalesTrans.GetInt32(4);
                        spoiltDto.StockUnitLeft = drSalesTrans.IsDBNull(5) ? null : (Int32?)drSalesTrans.GetInt32(5);
                        spoiltDto.IsSyncReady = drSalesTrans.GetBoolean(6);
                        spoiltDto.SyncStatus = drSalesTrans.GetInt32(7);
                        spoiltDto.SpoilDetails = drSalesTrans.IsDBNull(8) ? null : drSalesTrans.GetString(8);
                        spoiltDto.ReasonSyncFailed = drSalesTrans.IsDBNull(9) ? null : drSalesTrans.GetString(9);
                        spoiltDto.ReportedBy = drSalesTrans.IsDBNull(10) ? null : drSalesTrans.GetString(10);
                        spoiltDto.SyncRefCreatedOn = drSalesTrans.IsDBNull(11) ? null : (DateTime?)drSalesTrans.GetDateTime(11);
                        spoiltDto.SyncRefModifiedOn = drSalesTrans.IsDBNull(12) ? null : (DateTime?)drSalesTrans.GetDateTime(12);
                        spoiltDto.MachineName = drSalesTrans.IsDBNull(13) ? null : drSalesTrans.GetString(13);
                        spoiltDto.SyncRefNo = drSalesTrans.IsDBNull(14) ? null : drSalesTrans.GetString(14);
                        spoiltDto.SyncDate = drSalesTrans.IsDBNull(15) ? null : (DateTime?)drSalesTrans.GetDateTime(15);
                        spoiltDto.SyncFailedCount = drSalesTrans.GetInt32(16);
                        spoiltDto.RefCreatedDate = drSalesTrans.IsDBNull(17) ? null : (DateTime?)drSalesTrans.GetDateTime(17);
                        spoiltDto.RefModifiedDate = drSalesTrans.IsDBNull(18) ? null : (DateTime?)drSalesTrans.GetDateTime(18);
                        spoiltDto.Cost = drSalesTrans.IsDBNull(19) ? 0 : drSalesTrans.GetDouble(19);
                        spoiltDto.CreatedOnUtc = drSalesTrans.GetDateTime(20);
                        spoiltDto.ModifiedOnUtc = drSalesTrans.IsDBNull(21) ? null : (DateTime?)drSalesTrans.GetDateTime(21);
                        //drSalesTrans.GetBytes(23, 0, spoiltDto.RowVersion, 0, 18);
                        txnSpoilList.Add(spoiltDto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return txnSpoilList;
        }

        public IList<TransactionDto> RetrieveUnSyncedSales(String modifiedDate, String createddate, Int32 rowLimit)
        {
            IList<TransactionDto> oList = null;
            TransactionDto salesTransaction = null;
            DBHelper.Parameters[] colParameters = null;

            try
            {
                colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@EntryDate", createddate),
                    new DBHelper.Parameters("@ROWLIMIT", rowLimit) ,
                }; // Structure Parameter Array


                using (IDataReader drSalesTrans = this.ExecuteReader(CommandType.StoredProcedure, "[Sync].[Sp_RetrieveUnSyncedSales]", colParameters))
                {
                    oList = new List<TransactionDto>();

                    //Reading the reader one by one and setting into requestTypeInfo.
                    while (drSalesTrans.Read())
                    {
                        salesTransaction = new TransactionDto();
                        salesTransaction.TransactionRefNo = drSalesTrans.IsDBNull(0) ? null : drSalesTrans.GetString(0);
                        salesTransaction.StockRefNo = drSalesTrans.IsDBNull(1) ? null : drSalesTrans.GetString(1);
                        salesTransaction.StockItemNo = drSalesTrans.IsDBNull(2) ? null : drSalesTrans.GetString(2);
                        salesTransaction.StockDetails = drSalesTrans.IsDBNull(3) ? null : drSalesTrans.GetString(3);
                        salesTransaction.StockCategoryRefNo = drSalesTrans.IsDBNull(4) ? null : drSalesTrans.GetString(4);
                        salesTransaction.StockCategoryLine = drSalesTrans.IsDBNull(5) ? null : drSalesTrans.GetString(5);
                        salesTransaction.StockUnitPurchased = drSalesTrans.IsDBNull(6) ? 0 : drSalesTrans.GetInt32(6);
                        salesTransaction.StockUnitAmount = drSalesTrans.IsDBNull(7) ? 0 : drSalesTrans.GetDecimal(7);
                        salesTransaction.StockCostAmount = drSalesTrans.IsDBNull(8) ? 0 : drSalesTrans.GetDecimal(8);
                        salesTransaction.StockDiscountAmount = drSalesTrans.IsDBNull(9) ? 0 : drSalesTrans.GetDouble(9);
                        salesTransaction.RefCreatedDate = drSalesTrans.IsDBNull(10) ? null : (DateTime?)drSalesTrans.GetDateTime(10);
                        salesTransaction.CustomerName = drSalesTrans.IsDBNull(11) ? null : drSalesTrans.GetString(11);
                        salesTransaction.CustomerEmail = drSalesTrans.IsDBNull(12) ? null : drSalesTrans.GetString(12);
                        salesTransaction.CustomerAddress = drSalesTrans.IsDBNull(13) ? null : drSalesTrans.GetString(13);
                        salesTransaction.CustomerTel = drSalesTrans.IsDBNull(14) ? null : drSalesTrans.GetString(14);
                        if (drSalesTrans.IsDBNull(15))
                        {
                            salesTransaction.CustomerGender = null;
                        }
                        else
                        {
                            if (drSalesTrans.GetInt32(15) == 1)
                            {
                                salesTransaction.CustomerGender = "F";// FEMALE = 1
                            }

                            else if (drSalesTrans.GetInt32(15) == 2)
                            {
                                salesTransaction.CustomerGender = "M";// MALE = 2
                            }
                            else
                            {
                                salesTransaction.CustomerGender = null;
                            }

                        }
                        salesTransaction.Barcode = drSalesTrans.IsDBNull(16) ? null : drSalesTrans.GetString(16);
                        salesTransaction.StockUnitLeft = drSalesTrans.IsDBNull(18) ? null : (int?)drSalesTrans.GetInt32(18);
                        salesTransaction.Cashier = drSalesTrans.GetString(19);
                        salesTransaction.StockReorderUnit = drSalesTrans.IsDBNull(20) ? 0 : (int)drSalesTrans.GetInt32(20);
                        salesTransaction.Remark = drSalesTrans.IsDBNull(21) ? null : drSalesTrans.GetString(21);
                        oList.Add(salesTransaction);
                    }
                }

                return oList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                colParameters = null;
            }
        }

        public void UpdateSyncTransaction(TransactionDto txn)
        {
            String tsql = @"[Sync].[SP_UpdateSyncTransaction]";
            try
            {
                var colParameters = new DBHelper.Parameters[]
                    {
                    new DBHelper.Parameters("@IsSyncReady", txn.IsSyncReady),
                    new DBHelper.Parameters("@SyncDate", txn.SyncDate),
                    new DBHelper.Parameters("@SyncRefNo", txn.SyncRefNo),
                    new DBHelper.Parameters("@SyncRefCreatedOn", txn.SyncRefCreatedOn),
                    new DBHelper.Parameters("@SyncRefModifiedOn", txn.SyncRefModifiedOn),
                    new DBHelper.Parameters("@ReasonSyncFailed", txn.ReasonSyncFailed ?? string.Empty),
                     new DBHelper.Parameters("@SyncFailedCount", txn.SyncFailedCount),
                    new DBHelper.Parameters("@Id", txn.Id)
                    };

                this.ExecuteNonQuery(CommandType.StoredProcedure, tsql, colParameters);
            }
            catch (DBConcurrencyException)
            {
            }
        }

        public List<TransactionDto> GetRangeUnsyncedTxn(String count, int pending, bool syncReady)
        {
            String tsql = @"SELECT TOP " + count + @"  txn.* FROM [Sync].[Transactions] txn
                            WHERE txn.SyncStatus = @p0 AND txn.IsSyncReady = @p1";

            List<TransactionDto> txnDtoList = new List<TransactionDto>();

            try
            {
                // Structure Parameter Array
                var colParameters = new DBHelper.Parameters[]
                {
                    //new DBHelper.Parameters("@p0", count),
                    new DBHelper.Parameters("@p0", pending),
                    new DBHelper.Parameters("@p1", syncReady)
                };

                using (IDataReader drSalesTrans = this.ExecuteReader(CommandType.Text, tsql, colParameters))
                {
                    while (drSalesTrans.Read())
                    {
                        var salesTransaction = new TransactionDto();

                        salesTransaction.Id = drSalesTrans.GetGuid(0);
                        salesTransaction.TransactionRefNo = drSalesTrans.IsDBNull(1) ? null : drSalesTrans.GetString(1);
                        salesTransaction.TransactionRefNo = drSalesTrans.IsDBNull(2) ? null : drSalesTrans.GetString(2);
                        salesTransaction.CurrencyCode = drSalesTrans.IsDBNull(3) ? null : drSalesTrans.GetString(3);
                        salesTransaction.Cashier = drSalesTrans.GetString(4);
                        salesTransaction.MachineName = drSalesTrans.GetString(5);
                        salesTransaction.SyncDate = drSalesTrans.IsDBNull(6) ? null : (DateTime?)drSalesTrans.GetDateTime(6);
                        salesTransaction.SyncStatus = drSalesTrans.GetInt32(7);
                        salesTransaction.IsSyncReady = drSalesTrans.IsDBNull(8) ? false : drSalesTrans.GetBoolean(8);
                        salesTransaction.StockItemNo = drSalesTrans.IsDBNull(9) ? null : drSalesTrans.GetString(9);
                        salesTransaction.StockRefNo = drSalesTrans.IsDBNull(10) ? null : drSalesTrans.GetString(10);
                        salesTransaction.StockItemCode = drSalesTrans.IsDBNull(11) ? null : drSalesTrans.GetString(11);
                        salesTransaction.StockDetails = drSalesTrans.IsDBNull(12) ? null : drSalesTrans.GetString(12);
                        salesTransaction.StockCategoryRefNo = drSalesTrans.IsDBNull(13) ? null : drSalesTrans.GetString(13);
                        salesTransaction.StockCategoryLine = drSalesTrans.IsDBNull(14) ? null : drSalesTrans.GetString(14);
                        salesTransaction.StockUnitLeft = drSalesTrans.IsDBNull(15) ? null : (int?)drSalesTrans.GetInt32(15);
                        salesTransaction.StockUnitPurchased = drSalesTrans.IsDBNull(16) ? 0 : drSalesTrans.GetInt32(16);
                        salesTransaction.StockUnitAmount = drSalesTrans.IsDBNull(17) ? 0 : drSalesTrans.GetDecimal(17);
                        salesTransaction.StockCostAmount = drSalesTrans.IsDBNull(18) ? 0 : drSalesTrans.GetDecimal(18);
                        salesTransaction.StockDiscountAmount = drSalesTrans.IsDBNull(19) ? 0 : drSalesTrans.GetDouble(19);
                        salesTransaction.RefModifiedDate = drSalesTrans.IsDBNull(20) ? null : (DateTime?)drSalesTrans.GetDateTime(20);
                        salesTransaction.RefCreatedDate = drSalesTrans.IsDBNull(21) ? null : (DateTime?)drSalesTrans.GetDateTime(21);
                        salesTransaction.CustomerName = drSalesTrans.IsDBNull(22) ? null : drSalesTrans.GetString(22);
                        salesTransaction.CustomerEmail = drSalesTrans.IsDBNull(23) ? null : drSalesTrans.GetString(23);
                        salesTransaction.CustomerAddress = drSalesTrans.IsDBNull(24) ? null : drSalesTrans.GetString(24);
                        salesTransaction.CustomerTel = drSalesTrans.IsDBNull(25) ? null : drSalesTrans.GetString(25);

                        if (drSalesTrans.IsDBNull(26))
                        {
                            salesTransaction.CustomerGender = null;
                        }
                        else
                        {
                            if (drSalesTrans.GetInt32(26) == 1)
                            {
                                salesTransaction.CustomerGender = "F";// FEMALE = 1
                            }

                            else if (drSalesTrans.GetInt32(26) == 2)
                            {
                                salesTransaction.CustomerGender = "M";// MALE = 2
                            }
                            else
                            {
                                salesTransaction.CustomerGender = null;
                            }
                        }

                        salesTransaction.CustomerDOB = drSalesTrans.IsDBNull(27) ? null : (DateTime?)drSalesTrans.GetDateTime(27);
                        salesTransaction.Barcode = drSalesTrans.IsDBNull(28) ? null : drSalesTrans.GetString(28);
                        salesTransaction.TaxAmount = drSalesTrans.GetDecimal(29);
                        salesTransaction.CreatedOnUtc = drSalesTrans.GetDateTime(30);
                        salesTransaction.ModifiedOnUtc = drSalesTrans.IsDBNull(31) ? null : (DateTime?)drSalesTrans.GetDateTime(31);
                        salesTransaction.SyncFailedCount = drSalesTrans.IsDBNull(34) ? 0 : drSalesTrans.GetInt32(34);
                        salesTransaction.SyncRefNo = drSalesTrans.IsDBNull(35) ? null : drSalesTrans.GetString(35);
                        salesTransaction.SyncRefCreatedOn = drSalesTrans.IsDBNull(36) ? null : (DateTime?)drSalesTrans.GetDateTime(36);
                        salesTransaction.SyncRefModifiedOn = drSalesTrans.IsDBNull(37) ? null : (DateTime?)drSalesTrans.GetDateTime(37);
                        salesTransaction.StockReorderUnit = drSalesTrans.IsDBNull(38) ? 0 : (int)drSalesTrans.GetInt32(38);

                        txnDtoList.Add(salesTransaction);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return txnDtoList;
        }

        public TransactionDto GetTransactionDetailBy(Guid? orderDetailUId, Guid? orderUId)
        {
            TransactionDto salesTransaction = null;
            DBHelper.Parameters[] colParameters = null;

            try
            {
                // Structure Parameter Array
                colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@OrderDetailUId", orderDetailUId),
                    new DBHelper.Parameters("@OrderUId", orderUId),
                };

                using (IDataReader drSalesTrans = this.ExecuteReader(CommandType.StoredProcedure, "[sync].[Sp_RetrieveSalesBy]", colParameters))
                {
                    //Reading the reader one by one and setting into requestTypeInfo.
                    if (drSalesTrans.Read())
                    {
                        salesTransaction = new TransactionDto();
                        salesTransaction.TransactionRefNo = drSalesTrans.IsDBNull(0) ? null : drSalesTrans.GetString(0);
                        salesTransaction.StockRefNo = drSalesTrans.IsDBNull(1) ? null : drSalesTrans.GetString(1);
                        salesTransaction.StockItemNo = drSalesTrans.IsDBNull(2) ? null : drSalesTrans.GetString(2);
                        salesTransaction.StockDetails = drSalesTrans.IsDBNull(3) ? null : drSalesTrans.GetString(3);
                        salesTransaction.StockCategoryRefNo = drSalesTrans.IsDBNull(4) ? null : drSalesTrans.GetString(4);
                        salesTransaction.StockCategoryLine = drSalesTrans.IsDBNull(5) ? null : drSalesTrans.GetString(5);
                        salesTransaction.StockUnitPurchased = drSalesTrans.IsDBNull(6) ? 0 : drSalesTrans.GetInt32(6);
                        salesTransaction.StockUnitAmount = drSalesTrans.IsDBNull(7) ? 0 : drSalesTrans.GetDecimal(7);
                        salesTransaction.StockCostAmount = drSalesTrans.IsDBNull(8) ? 0 : drSalesTrans.GetDecimal(8);
                        salesTransaction.StockDiscountAmount = drSalesTrans.IsDBNull(9) ? 0 : drSalesTrans.GetDouble(9);
                        salesTransaction.RefCreatedDate = drSalesTrans.IsDBNull(10) ? null : (DateTime?)drSalesTrans.GetDateTime(10);
                        salesTransaction.CustomerName = drSalesTrans.IsDBNull(11) ? null : drSalesTrans.GetString(11);
                        salesTransaction.CustomerEmail = drSalesTrans.IsDBNull(12) ? null : drSalesTrans.GetString(12);
                        salesTransaction.CustomerAddress = drSalesTrans.IsDBNull(13) ? null : drSalesTrans.GetString(13);
                        salesTransaction.CustomerTel = drSalesTrans.IsDBNull(14) ? null : drSalesTrans.GetString(14);
                        salesTransaction.CustomerGender = drSalesTrans.IsDBNull(15) ? null : drSalesTrans.GetString(15);
                        salesTransaction.Barcode = drSalesTrans.IsDBNull(16) ? null : drSalesTrans.GetString(16);
                        salesTransaction.RefModifiedDate = drSalesTrans.IsDBNull(17) ? null : (DateTime?)drSalesTrans.GetDateTime(17);
                        salesTransaction.StockUnitLeft = drSalesTrans.IsDBNull(18) ? null : (int?)drSalesTrans.GetInt32(18);
                        salesTransaction.Cashier = drSalesTrans.GetString(19);
                        salesTransaction.StockReorderUnit = drSalesTrans.IsDBNull(20) ? 0 : (int)drSalesTrans.GetInt32(20);
                    }
                }

                return salesTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                colParameters = null;
            }
        }

        public IList<SpoilDto> RetrieveUnSyncedSpoils(string createdDate)
        {
            IList<SpoilDto> sList = null;
            SpoilDto spoilTransaction = null;
            DBHelper.Parameters[] colParameters = null;

            try
            {
                colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@EntryDate", createdDate),
                };


                using (IDataReader spoilTrans = this.ExecuteReader(CommandType.StoredProcedure, "[Sync].[Sp_retrieveunsyncedspoils]", colParameters))
                {
                    sList = new List<SpoilDto>();

                    //Reading the reader one by one and setting into requestTypeInfo.
                    while (spoilTrans.Read())
                    {
                        spoilTransaction = new SpoilDto();
                        spoilTransaction.TransactionRefNo = spoilTrans.IsDBNull(0) ? null : spoilTrans.GetString(0);
                        spoilTransaction.StockRefNo = spoilTrans.IsDBNull(1) ? 0 : (int)spoilTrans.GetInt32(1);
                        spoilTransaction.SpoilDetails = spoilTrans.IsDBNull(2) ? null : spoilTrans.GetString(2);
                        spoilTransaction.StockDetails = spoilTrans.IsDBNull(3) ? null : spoilTrans.GetString(3);
                        spoilTransaction.ReportedBy = spoilTrans.IsDBNull(4) ? null : spoilTrans.GetString(4);
                        spoilTransaction.StockUnit = spoilTrans.IsDBNull(5) ? 0 : (int)spoilTrans.GetInt32(5);
                        spoilTransaction.IsDeleted = spoilTrans.IsDBNull(6) ? false : spoilTrans.GetBoolean(6);
                        spoilTransaction.RefCreatedDate = spoilTrans.IsDBNull(7) ? null : (DateTime?)spoilTrans.GetDateTime(7);
                        spoilTransaction.RefModifiedDate = spoilTrans.IsDBNull(8) ? null : (DateTime?)spoilTrans.GetDateTime(8);
                        spoilTransaction.Cost = spoilTrans.IsDBNull(9) ? null : (Double?)spoilTrans.GetDecimal(9);
                        sList.Add(spoilTransaction);
                    }
                }

                return sList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                colParameters = null;
            }
        }

        public SpoilDto GetUnSyncedSpoilDetailBy(Guid? Id)
        {
            SpoilDto spoilTransaction = null;
            DBHelper.Parameters[] colParameters = null;

            try
            {
                colParameters = new DBHelper.Parameters[]
                {
                    new DBHelper.Parameters("@Id", Id),
                };

                using (IDataReader spoilTrans = this.ExecuteReader(CommandType.StoredProcedure, "[Sync].[Sp_retrieveunsyncedspoils]", colParameters))
                {

                    //Reading the reader one by one and setting into requestTypeInfo.
                    while (spoilTrans.Read())
                    {
                        spoilTransaction = new SpoilDto();
                        spoilTransaction.TransactionRefNo = spoilTrans.IsDBNull(0) ? null : spoilTrans.GetString(0);
                        spoilTransaction.StockRefNo = spoilTrans.IsDBNull(1) ? 0 : (int)spoilTrans.GetInt32(1);
                        spoilTransaction.SpoilDetails = spoilTrans.IsDBNull(2) ? null : spoilTrans.GetString(2);
                        spoilTransaction.StockDetails = spoilTrans.IsDBNull(3) ? null : spoilTrans.GetString(3);
                        spoilTransaction.ReportedBy = spoilTrans.IsDBNull(4) ? null : spoilTrans.GetString(4);
                        spoilTransaction.StockUnit = spoilTrans.IsDBNull(5) ? 0 : (int)spoilTrans.GetInt32(5);
                        spoilTransaction.IsDeleted = spoilTrans.IsDBNull(6) ? false : spoilTrans.GetBoolean(6);
                        spoilTransaction.RefCreatedDate = spoilTrans.IsDBNull(7) ? null : (DateTime?)spoilTrans.GetDateTime(7);
                        spoilTransaction.RefModifiedDate = spoilTrans.IsDBNull(8) ? null : (DateTime?)spoilTrans.GetDateTime(8);
                        spoilTransaction.Cost = spoilTrans.IsDBNull(9) ? null : (double?)spoilTrans.GetDouble(9);
                    }
                }

                return spoilTransaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                colParameters = null;
            }
        }


    }
}