using Ipos.Sync.Core.Models;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
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
    public class SpoilDependency
    {
        private readonly SqlTableDependency<SpoilDto> tableDependency;
        private readonly IStoreDataProvider _storeProvider;
        private readonly SpoilSyncService _spoilSyncSvc;

        public SpoilDependency(SpoilSyncService spoilSyncSvc, IStoreDataProvider storeProvider)
        {
            _spoilSyncSvc = spoilSyncSvc;
            var connectionString = ConfigurationManager.AppSettings["DATA.CONNECTIONSTRING"];
            var providerName = ConfigurationManager.AppSettings["DATA.PROVIDER"];
            _storeProvider = storeProvider;

            var mapper = new ModelToTableMapper<SpoilDto>();
            mapper.AddMapping(c => c.StockRefNo, "Product_Id");

            tableDependency = new SqlTableDependency<SpoilDto>(connectionString, "[dbo].[Spoil]", mapper);
            tableDependency.TraceListener = new TextWriterTraceListener(Console.Out);
            tableDependency.OnChanged += tableDependency_OnChanged;
            tableDependency.OnError += tableDependency_OnError;
            tableDependency.TraceListener = new TextWriterTraceListener(File.Create("c:\\Iposlogs\\spoil-log-output.txt"));
        }

        private void tableDependency_OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<SpoilDto> e)
        {
            SyncSpoilChanges(e.Entity, e.ChangeType);
        }

        private void tableDependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            Log.Logger.Error(e.Message);
        }

        private void SyncSpoilChanges(SpoilDto spoilDto, TableDependency.Enums.ChangeType changeType)
        {
            var unsyncedSpoil = GetUnSyncSpoilBy(spoilDto.SpoilId);

            if (unsyncedSpoil == null && changeType != ChangeType.Delete)
                return;

            switch (changeType)
            {
                case ChangeType.Delete:
                    {
                        String spoilId = spoilDto.SpoilId.ToString();
                        var existingSpoil = _spoilSyncSvc.FirstOrDefault(t => t.TransactionRefNo == spoilId);

                        if (existingSpoil != null)
                        {
                            existingSpoil.IsDeleted = true;
                            Spoil.Extend(unsyncedSpoil, existingSpoil);
                            _spoilSyncSvc.Update(existingSpoil);
                        }
                        break;
                    }
                case TableDependency.Enums.ChangeType.Insert:
                    {
                        unsyncedSpoil.RefModifiedDate = unsyncedSpoil.RefCreatedDate;

                        var dbTrt = _spoilSyncSvc.FirstOrDefault(t => t.TransactionRefNo == unsyncedSpoil.TransactionRefNo);

                        if (dbTrt == null)
                        {
                            unsyncedSpoil.ModifiedOnUtc = unsyncedSpoil.CreatedOnUtc = DateTime.UtcNow;
                            _spoilSyncSvc.Add(unsyncedSpoil);
                        }
                        else
                        {
                            Spoil.Extend(unsyncedSpoil, dbTrt);
                            _spoilSyncSvc.Update(dbTrt);
                        }
                        break;
                    }
                case TableDependency.Enums.ChangeType.Update:
                    {
                        if (spoilDto.IsDeleted)
                            goto case ChangeType.Delete;

                        var dbTrt = _spoilSyncSvc.FirstOrDefault(t => t.TransactionRefNo == unsyncedSpoil.TransactionRefNo);

                        if (dbTrt == null)
                        {
                            unsyncedSpoil.CreatedOnUtc = DateTime.UtcNow;
                            unsyncedSpoil.ModifiedOnUtc = DateTime.UtcNow;
                            Log.Warning(String.Format("Spoil with TransactionRefNo ({0}) couldn't be retreived for update.", unsyncedSpoil.TransactionRefNo));
                            Log.Information(String.Format("Adding Spoil with TransactionRefNo ({0}) to sync table as newly created.", unsyncedSpoil.StockRefNo));
                            _spoilSyncSvc.Add(unsyncedSpoil);
                        }
                        else
                        {
                            Spoil.Extend(unsyncedSpoil, dbTrt);
                            _spoilSyncSvc.Update(dbTrt);
                        }

                        break;
                    }
                case TableDependency.Enums.ChangeType.None:
                default:
                    break;
            }
        }

        private Spoil GetUnSyncSpoilBy(Guid? id)
        {
            var searchedSpoil = _storeProvider.GetUnSyncedSpoilDetailBy(id);

            if (searchedSpoil == null)
                return null;

            var trt = Spoil.Create(searchedSpoil);
            trt.MachineName = Environment.MachineName;
            return trt;
        }

        public void Start()
        {
            tableDependency.Start();
        }
        public void Stop()
        {
            tableDependency.Stop();
            tableDependency.Dispose();
        }
    }
}