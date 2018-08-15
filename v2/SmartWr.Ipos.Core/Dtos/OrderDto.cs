using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class OrderDto : BaseEntity
    {
        public Decimal TotalTodaySales { get; set; }
        public Decimal TotalWeekSales { get; set; }
        public Int32 TotalTransactionCount { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
