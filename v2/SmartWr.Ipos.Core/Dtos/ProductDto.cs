using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class ProductDto : BaseEntity
    {
        public String FullDescription
        {
            get
            {
                return String.Format("{0} - {1}", Name, Description);
            }
        }
        public string Name { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? Category_Id { get; set; }
        public int ProductId { get; set; }
        public int? TotalCount { get; set; }
        public bool IsDiscountable { get; set; }
        public string Notes { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Description { get; set; }
        public bool? CanExpire { get; set; }
        public string Barcode { get; set; }
        public int? ReorderLevel { get; set; }
        public Guid ProductUId { get; set; }


        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}