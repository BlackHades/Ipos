using Ipos.Sync.Core.Models;
using Ipos.Sync.Core.Models.Enums;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Db;
using Ipos.Sync.StoreDataProviders.Dtos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency.Enums;
using TableDependency.Mappers;
using TableDependency.SqlClient;

namespace Ipos.Sync.Core.Components.DbBroker
{
    //https://tabledependency.codeplex.com/wikipage?title=SqlTableDependency
    //ALTER DATABASE dbname SET ENABLE_BROKER
    //alter authorization on database::[dbname] to [loginname]
    public class TransactionDependency
    {
        private readonly SqlTableDependency<OrderDetailDto> tableDependency;
        private readonly IStoreDataProvider _storeProvider;
        private readonly TransactionSyncService _trtSyncSvc;

        public TransactionDependency(TransactionSyncService trtSyncSvc, IStoreDataProvider storeProvider)
        {


            _trtSyncSvc = trtSyncSvc;
            var connectionString = ConfigurationManager.AppSettings["DATA.CONNECTIONSTRING"];
            var providerName = ConfigurationManager.AppSettings["DATA.PROVIDER"];
            _storeProvider = storeProvider;

            var mapper = new ModelToTableMapper<OrderDetailDto>();
            mapper.AddMapping(c => c.OrderDetailUId, "OrderDetailUId");
            mapper.AddMapping(c => c.Order_UId, "Order_UId");
            //mapper.AddMapping(c => c.Product_Id, "Product_Id");
            //mapper.AddMapping(c => c.CostPrice, "CostPrice");
            //mapper.AddMapping(c => c.Discount, "Discount");
            //mapper.AddMapping(c => c.Price, "Price");
            //mapper.AddMapping(c => c.Quantiy, "Quantiy");
            //mapper.AddMapping(c => c.EntryDate, "EntryDate");

            tableDependency = new SqlTableDependency<OrderDetailDto>(connectionString, "[dbo].[OrderDetails]", mapper);
            tableDependency.TraceListener = new TextWriterTraceListener(Console.Out);
            tableDependency.OnChanged += tableDependency_OnChanged;
            tableDependency.OnError += tableDependency_OnError;
            tableDependency.TraceListener = new TextWriterTraceListener(File.Create("c:\\Iposlogs\\transactionlogoutput.txt"));
        }

        void tableDependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            //Write email adminsitrators code here
            Log.Logger.Error(e.Message);
        }

        public void Start()
        {
            tableDependency.Start();
        }

        void tableDependency_OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<OrderDetailDto> e)
        {
            var orderDetail = e.Entity;
            SyncTransactionChanges(orderDetail, e.ChangeType);
        }

        private void SyncTransactionChanges(OrderDetailDto trtItem, TableDependency.Enums.ChangeType changeType)
        {
            var trt = GetSyncTransactionBy(trtItem.OrderDetailUId, null);

            if (trt == null && changeType != ChangeType.Delete)
                return;

            switch (changeType)
            {
                case ChangeType.Delete:
                    {
                        String ordtUId = trtItem.OrderDetailUId.ToString();
                        var dbTrt = _trtSyncSvc.FirstOrDefault(t => ordtUId.Equals(t.StockRefNo, StringComparison.InvariantCultureIgnoreCase));

                        if (dbTrt != null)
                        {
                            dbTrt.IsDeleted = true;
                            Transaction.Extend(trt, dbTrt);
                            _trtSyncSvc.Update(dbTrt);
                        }

                        break;
                    }
                case TableDependency.Enums.ChangeType.Insert:
                    {
                        trt.RefModifiedDate = trt.RefCreatedDate;

                        var dbTrt = _trtSyncSvc.FirstOrDefault(t => t.StockRefNo == trt.StockRefNo);

                        if (dbTrt == null)
                        {
                            trt.ModifiedOnUtc = trt.CreatedOnUtc = DateTime.UtcNow;
                            _trtSyncSvc.Add(trt);
                        }
                        else
                        {
                            Transaction.Extend(trt, dbTrt);
                            _trtSyncSvc.Update(dbTrt);
                        }
                        break;
                    }
                case TableDependency.Enums.ChangeType.Update:
                    {
                        if (trtItem.IsDeleted)
                        {
                            goto case ChangeType.Delete;
                        }

                        trt.IsDeleted = true;

                        var dbTrt = _trtSyncSvc.FirstOrDefault(t => t.StockRefNo == trt.StockRefNo);

                        if (dbTrt == null)
                        {
                            trt.CreatedOnUtc = DateTime.UtcNow;
                            trt.ModifiedOnUtc = DateTime.UtcNow;
                            Log.Warning(String.Format("Transaction with stock ref no ({0}) couldn't be retreive to update", trt.StockRefNo));
                            Log.Information(String.Format("Adding transaction with stock ref no ({0}) to sync table as newly created", trt.StockRefNo));
                            _trtSyncSvc.Add(trt);
                        }
                        else
                        {
                            Transaction.Extend(trt, dbTrt);
                            _trtSyncSvc.Update(dbTrt);
                        }

                        break;
                    }
                case TableDependency.Enums.ChangeType.None:
                default:
                    break;
            }
        }

        private Transaction GetSyncTransactionBy(Guid? orderDetailUId, Guid? orderUId)
        {
            var searchedSales = _storeProvider.GetTransactionDetailBy(orderDetailUId, orderUId);

            if (searchedSales == null)
                return null;

            var trt = Transaction.Create(searchedSales);
            trt.CurrencyCode = CurrencyCodeHelper.NGN;
            trt.MachineName = Environment.MachineName;
            return trt;
        }

        public void Stop()
        {
            tableDependency.Stop();
            tableDependency.Dispose();
        }
    }
}
