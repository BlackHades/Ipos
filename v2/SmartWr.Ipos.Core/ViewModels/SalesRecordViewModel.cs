using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class SalesRecordViewModel
    {
        public String Remark { get; set; }
        public Decimal SubTotal { get; set; }
        public Guid OrderUId { get; set; }
        public string Status { get; set; }
        public decimal? Total { get; set; }
        public int? TotalItemsBought { get; set; }
        public decimal? Profit { get; set; }
        public string PaymentMethod { get; set; }
        public string CreatedDate { get; set; }
        public string CustomerName { get; set; }
        public bool? Discounted { get; set; }
        public double? Discount { get; set; }

    }
}