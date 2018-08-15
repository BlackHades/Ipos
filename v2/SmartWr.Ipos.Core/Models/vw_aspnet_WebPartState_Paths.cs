using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class vw_aspnet_WebPartState_Paths
    {
        public Guid ApplicationId { get; set; }
        public Guid PathId { get; set; }
        public string Path { get; set; }
        public string LoweredPath { get; set; }
    }
}
