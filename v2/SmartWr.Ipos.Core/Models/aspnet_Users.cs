using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_Users:BaseEntity
    {
        public aspnet_Users()
        {
            this.aspnet_PersonalizationPerUser = new List<aspnet_PersonalizationPerUser>();
            this.Audits = new List<Audit>();
            this.BulkSMS = new List<BulkSM>();
            this.Orders = new List<Order>();
            this.Products = new List<Product>();
            this.aspnet_Roles = new List<aspnet_Roles>();
        }

        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
        public virtual aspnet_Applications aspnet_Applications { get; set; }
        public virtual aspnet_Membership aspnet_Membership { get; set; }
        public virtual ICollection<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual aspnet_Profile aspnet_Profile { get; set; }
        public virtual ICollection<Audit> Audits { get; set; }
        public virtual ICollection<BulkSM> BulkSMS { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<aspnet_Roles> aspnet_Roles { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
