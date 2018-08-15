using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class RecentItemBoughtDto : BaseEntity
    {
        public Guid OrderUId { get; set; }
        public Int32? ItemCount { get; set; }
        public Decimal TotalSellingPrice { get; set; }
        public Decimal TotalCostPrice { get; set; }
        public DateTime TransactionDate { get; set; }
        public String StaffName { get; set; }

        public String TransDate
        {
            get
            {
                return this.TransactionDate.ToShortDateString();
            }
        }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}