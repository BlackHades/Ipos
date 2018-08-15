using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class HourlySalesOrder
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Hour { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
    }
}
