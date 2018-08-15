using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_PersonalizationPerUser
    {
        public Guid Id { get; set; }
        public Nullable<Guid> PathId { get; set; }
        public Nullable<Guid> UserId { get; set; }
        public byte[] PageSettings { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public virtual aspnet_Paths aspnet_Paths { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
