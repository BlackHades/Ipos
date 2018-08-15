using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class OrderDetailDto : BaseEntity
    {
        public String Remark { get; set; }
        public Decimal? UnitCost { get; set; }
        public Decimal? CurrentPrice { get; set; }
        public int? Status { get; set; }
        public Double? Total { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? PaymentMethod { get; set; }
        public string CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public bool? IsDiscounted { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? CostPrice { get; set; }
        public Double? Price { get; set; }
        public Guid OrderUId { get; set; }
        public Guid OrderDetailId { get; set; }
        public double? Discount { get; set; }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }

        public String Description { get; set; }
    }

}