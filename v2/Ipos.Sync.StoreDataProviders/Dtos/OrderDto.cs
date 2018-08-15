using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipos.Sync.StoreDataProviders.Dtos
{
    public class OrderDto
    {
        public Guid OrderUId { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Guid User_Id { get; set; }
        public Nullable<bool> IsDiscounted { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Remark { get; set; }
        public Nullable<int> OrderStatus { get; set; }
        public Nullable<int> Customer_UId { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
    }
}
