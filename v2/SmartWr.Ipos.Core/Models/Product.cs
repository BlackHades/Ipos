using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Product : BaseEntity
    {
        public Product()
        {
            this.OrderDetails = new List<OrderDetail>();
            this.Categories = new List<Category>();
        }

        public Guid ProductUId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<Guid> Insert_UId { get; set; }
        public Nullable<Guid> Update_UId { get; set; }
        public string PhotoURL { get; set; }
        public string Extention { get; set; }
        public string FileName { get; set; }
        public bool IsDiscountable { get; set; }
        public string Barcode { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<int> ReorderLevel { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> FileSize { get; set; }
        public int ProductId { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public bool CanExpire { get; set; }
        public Nullable<int> Category_UId { get; set; }
        public Nullable<bool> IsDiscontinued { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
