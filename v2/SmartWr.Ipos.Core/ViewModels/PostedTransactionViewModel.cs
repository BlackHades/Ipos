namespace SmartWr.Ipos.Core.ViewModels
{
    public class PostedTransactionViewModel
    {
        public PostedProduct[] Products { get; set; }
        public string Remarks { get; set; }
        public int Quantity { get; set; }

    }
    public class PostedProduct
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string CreatedDate { get; set; }
        public string Remarks { get; set; }
    }
}