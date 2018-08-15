using Ipos.Sync.Core.ApiClient;
using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Helpers;
using Ipos.Sync.Core.Models;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
using IposAnalytics.Logic.DataContracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Services
{
    public class TransactionSyncService : Service<Transaction, Guid>
    {
        private readonly SyncApiClient _apiClient;
        private ISyncStoreDataProvider _mssqlProvider;
        public TransactionSyncService(SyncApiClient apiClient,
              ISyncStoreDataProvider mssqlProvider,
            IUnitOfWork<Guid> unitOfWork)
            : base(unitOfWork)
        {
            _apiClient = apiClient;
            _mssqlProvider = mssqlProvider;
        }
        public async Task<IEnumerable<TransactionDto>> SyncTransactionWithCloud(IEnumerable<TransactionDto> trtSale)
        {
            try
            {
                return await _apiClient.SendBulkSales(trtSale);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return null;
            }
        }

        //[Obsolete]
        //public async Task<TransactionDto> SyncTransactionWithCloud(TransactionDto trtSale)
        //{
        //    try
        //    {
        //        return await _apiClient.SendSingleSales(trtSale);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Information(ex.Message);
        //        return null;
        //    }
        //}

        public Transaction GetLastTransaction()
        {
            var dr = _mssqlProvider.GetLastTransaction();

            if (!dr.Read())
                return null;

            var txn = Transaction.Create(null);
            DataReaderHelper.DataReaderToObject(dr, txn);

            return txn;
        }

        public void UpdateSyncTransaction(Transaction txn)
        {
            this.Update(txn);
        }
    }
}
