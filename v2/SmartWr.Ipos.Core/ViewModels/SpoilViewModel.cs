using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class SpoilViewModel : BaseViewModel
    {
        public Guid SpoilId { get; set; }
        public string Title { get; set; }
        public string EntryDate { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }

    }
}