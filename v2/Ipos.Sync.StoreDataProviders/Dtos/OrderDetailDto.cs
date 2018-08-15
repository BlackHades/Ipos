using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Dtos
{
    public class OrderDetailDto
    {
        public Guid OrderDetailUId { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<Double> Discount { get; set; }
        public Nullable<Decimal> Price { get; set; }
        public Nullable<Int32> Quantiy { get; set; }
        public Nullable<Int32> Product_Id { get; set; }
        public Guid Order_UId { get; set; }
        public Decimal CostPrice { get; set; }
        public String Remarks { get; set; }
        public Int32? CreatedBy_Id { get; set; }
        public Int32? ModifiedBy_Id { get; set; }
        public DateTime ModifiedOnUtc { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
