using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class RestockDto : BaseEntity
    {

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public int Quantity { get; set; }

        public int ReorderLevel { get; set; }

        public String EntDate
        {
            get
            {
                return this.EntryDate.ToShortDateString();
            }
        }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }

        public Guid ProductUId { get; set; }
    }
}
