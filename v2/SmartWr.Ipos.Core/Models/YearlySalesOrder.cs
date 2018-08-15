using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class YearlySalesOrder
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
    }
}
