using SmartWr.WebFramework.Library.MiddleServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWr.Ipos.Core.Dtos
{
     public class TopSalesDto : BaseEntity
    {
         public string Name { get; set; }
         public int Quantity { get; set; }
         public decimal Total { get; set; }
        
         public override List<SmartWr.WebFramework.Library.Infrastructure.Validation.ValidationError> Validate()
         {
             throw new NotImplementedException();
         }
    }
}
