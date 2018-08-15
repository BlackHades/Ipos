using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class OrderDetail:BaseEntity
    {
        public Guid OrderDetailUId { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantiy { get; set; }
        public Nullable<int> Product_Id { get; set; }
        public Guid Order_UId { get; set; }
        public decimal CostPrice { get; set; }
        public string Remarks { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
