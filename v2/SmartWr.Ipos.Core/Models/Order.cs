using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Order:BaseEntity
    {
        public Order()
        {
            this.OrderDetails = new List<OrderDetail>();
        }

        public Guid OrderUId { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Guid User_Id { get; set; }
        public Nullable<bool> IsDiscounted { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Remark { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<int> Customer_UId { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
