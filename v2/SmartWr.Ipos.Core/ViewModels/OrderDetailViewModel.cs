using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class OrderDetailViewModel
    {
        public String FullDescription
        {
            get
            {
                return String.Format("{0} - {1}", ProductName, ProductDescription);
            }
        }


        public Decimal UnitCost { get; set; }
        public int ProductId { get; set; }
        public Guid OrderDetailUId { get; set; }
        public string ProductName { get; set; }
        public String ProductDescription { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Total { get; set; }
        public decimal SellPrice { get; set; }
        public double? Discount { get; set; }
        public Guid OrderUId { get; set; }
        public Guid Id { get; set; }
        public string EntryDate { get; set; }
        public int? TotalCnt { get; set; }

    }
}
