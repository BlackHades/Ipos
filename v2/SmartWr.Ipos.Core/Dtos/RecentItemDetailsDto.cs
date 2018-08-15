using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class RecentItemDetailsDto : BaseEntity
    {
        public string ProductName { get; set; }
        public string Staffname { get; set; }
        public Decimal? Price { get; set; }
        public Decimal? CostPrice { get; set; }
        public int? Quantity { get; set; }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}