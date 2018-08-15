using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class SalesRecordDto : BaseEntity
    {
        public Double Discount { get; set; }
        public Guid Order_UId { get; set; }
        public int? Status { get; set; }
        public int? TotalCount { get; set; }
        public Double? Total { get; set; }
        public int? TotalItemsBought { get; set; }
        public int? TotalPrice { get; set; }
        public decimal? Profit { get; set; }
        public int? PaymentMethod { get; set; }
        public string CreatedDate { get; set; }
        public string StaffName { get; set; }
        public decimal? SumTotal { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }

        public string OrderStatus { get; set; }
    }
}