using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Category : BaseEntity
    {
        public Category()
        {
            this.Products = new List<Product>();
            this.Products1 = new List<Product>();
        }

        public int CategoryUId { get; set; }
       
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Product> Products1 { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
