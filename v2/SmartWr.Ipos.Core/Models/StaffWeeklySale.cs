using System;

namespace SmartWr.Ipos.Core.Models
{
    public partial class StaffWeeklySale
    {
        public int Id { get; set; }
        public string MonthName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Week { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
    }
}
