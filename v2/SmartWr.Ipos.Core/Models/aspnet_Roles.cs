using System;
using System.Collections.Generic;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_Roles
    {
        public aspnet_Roles()
        {
            this.aspnet_Users = new List<aspnet_Users>();
        }

        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual ICollection<aspnet_Users> aspnet_Users { get; set; }
    }
}
