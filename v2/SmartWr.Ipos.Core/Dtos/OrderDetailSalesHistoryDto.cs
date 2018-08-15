using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class OrderDetailSalesHistoryDto :BaseEntity
    {
        public Guid OrderDetailUId { get; set; }

        public string CreatedDate { get; set; }
        public double? Discount { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? Total { get; set; }
       


        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
