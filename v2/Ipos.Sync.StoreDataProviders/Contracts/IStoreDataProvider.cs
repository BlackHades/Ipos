using Ipos.Sync.StoreDataProviders.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Contracts
{
    public interface IStoreDataProvider
    {
        TransactionDto GetTransactionDetailBy(Guid? orderDetailUId, Guid? orderUId);

        IList<TransactionDto> RetrieveUnSyncedSales(String modifiedDate, String createddate, Int32 rowLimit);

        IList<SpoilDto> RetrieveUnSyncedSpoils(String createdDate);

        SpoilDto GetUnSyncedSpoilDetailBy(Guid? Id);
    }
}