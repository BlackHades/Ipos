using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Audit: BaseEntity
    {
        public Guid AuditId { get; set; }
        public Nullable<int> AuditType { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<Guid> User_Id { get; set; }
        public string Description { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
