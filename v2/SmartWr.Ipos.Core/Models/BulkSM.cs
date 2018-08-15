using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class BulkSM:BaseEntity
    {
        public Guid SMSUId { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Recipients { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<int> MessageType { get; set; }
        public Nullable<int> DeliveryStatus { get; set; }
        public Guid User_Id { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
