using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_Profile
    {
        public Guid UserId { get; set; }
        public string PropertyNames { get; set; }
        public string PropertyValuesString { get; set; }
        public byte[] PropertyValuesBinary { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
