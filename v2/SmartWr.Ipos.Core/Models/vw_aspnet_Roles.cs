using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class vw_aspnet_Roles
    {
        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }
    }
}
