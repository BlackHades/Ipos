using Ipos.Sync.Core.ApiClient;
using Ipos.Sync.Core.Contracts;
using Ipos.Sync.Core.Helpers;
using Ipos.Sync.Core.Migrations;
using Ipos.Sync.Core.Models;
using Ipos.Sync.StoreDataProviders.Contracts;
using Ipos.Sync.StoreDataProviders.Dto;
using Ipos.Sync.StoreDataProviders.Ipos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Services
{
    public class SpoilSyncService : Service<Spoil, Guid>
    {
        private readonly SyncApiClient _apiClient;
        private ISyncStoreDataProvider _mssqlProvider;

        public SpoilSyncService(SyncApiClient apiClient,
            ISyncStoreDataProvider mssqlProvider,
            IUnitOfWork<Guid> unitOfWork) : base(unitOfWork)
        {
            _apiClient = apiClient;
            _mssqlProvider = mssqlProvider;
        }

        public Spoil GetLastRetrievedSpoil()
        {
            var dr = _mssqlProvider.GetLastSpoil();

            if (!dr.Read())
                return null;

            var spoil = new Spoil();
            DataReaderHelper.DataReaderToObject(dr, spoil);

            return spoil;
            //return this.UnitOfWork.Repository<Spoil>().SqlQuery("EXEC [Sync].[Sp_GetLastSpoil]").FirstOrDefault();
        }

        public async Task<List<SpoilDto>> SyncSpoilsWithCloud(List<SpoilDto> spoilList)
        {
            try
            {
                return await _apiClient.SendSpoil(spoilList);
            }
            catch (Exception ex)
            {
                Log.Information(ex.Message);
                return null;
            }
        }

        //public async Task<SpoilDto> SyncSpoilsWithCloud(SpoilDto spoilItem)
        //{
        //    try
        //    {
        //        return await _apiClient.SendSpoil(spoilItem);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Information(ex.Message);
        //        return null;
        //    }
        //}
    }
}