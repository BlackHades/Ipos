using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.Core.Models.Enums
{
    public enum SyncStatus
    {
        PENDING,
        COMPLETED,
        FAILED,
        REJECTED,
        RESYNC
    }
}
