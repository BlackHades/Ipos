using SmartWr.WebFramework.Library.MiddleServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWr.Ipos.Core.Dtos
{
     public class ExpiredProductsDto : BaseEntity
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public override List<SmartWr.WebFramework.Library.Infrastructure.Validation.ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
       
    }
}