using Hangfire;
using Ipos.Sync.Core.Components.DbBroker;
using Ipos.Sync.Core.Services;
using Ipos.Sync.StoreDataProviders.Contracts;
using System.Configuration;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Components.JobSchduler
{
    public class IposSyncService
    {
        private BackgroundJobServer _jobServer;
        private readonly TransactionDependency _trtDependency;
        private readonly SpoilDependency _spoilDependency;
        public IposSyncService(TransactionSyncService trtSyncSvc, SpoilSyncService spoilSync, IStoreDataProvider storeProvider)
        {
            _trtDependency = new TransactionDependency(trtSyncSvc, storeProvider);
            _spoilDependency = new SpoilDependency(spoilSync, storeProvider);
        }

        public void Start()
        {
            if (ConfigurationManager.AppSettings["DATA.PROVIDER"] == "System.Data.SqlClient")
            {
                _trtDependency.Start();
                _spoilDependency.Start();
            }

            _jobServer = new BackgroundJobServer();

        }

        public void Stop()
        {
            if (ConfigurationManager.AppSettings["DATA.PROVIDER"] == "System.Data.SqlClient")
            {
                _trtDependency.Stop();
                _spoilDependency.Stop();
            }

            _jobServer.Dispose();
        }
    }
}