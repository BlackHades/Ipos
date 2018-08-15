using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_PersonalizationAllUsers
    {
        public Guid PathId { get; set; }
        public byte[] PageSettings { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public virtual aspnet_Paths aspnet_Paths { get; set; }
    }
}
