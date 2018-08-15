using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class vw_aspnet_Profiles
    {
        public Guid UserId { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Nullable<int> DataSize { get; set; }
    }
}
