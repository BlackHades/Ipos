using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public decimal? SellPrice { get; set; }

        public decimal? CostPrice { get; set; }

        public int? Quantity { get; set; }

        public int? Category { get; set; }

        public bool IsDiscountable { get; set; }

        public string Notes { get; set; }
        
        public string ExpiryDate { get; set; }

        public string Description { get; set; }

        public bool CanExpire { get; set; }

        public string Barcode { get; set; }

        public int? ReorderLevel { get; set; }

        public Guid ProductUId { get; set; }

       
    }
}
