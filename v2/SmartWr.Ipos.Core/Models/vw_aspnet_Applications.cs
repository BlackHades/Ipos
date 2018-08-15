using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class vw_aspnet_Applications
    {
        public string ApplicationName { get; set; }
        public string LoweredApplicationName { get; set; }
        public Guid ApplicationId { get; set; }
        public string Description { get; set; }
    }
}
