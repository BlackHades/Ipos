using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class FaultyProductsDto : BaseEntity
    {
        public Guid SpoilId { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public DateTime? EntryDate { get; set; }
        public int Quantity { get; set; }

        public String EntDate
        {
            get
            {
                return EntryDate.HasValue ? EntryDate.Value.ToShortDateString() : "NA";
            }
        }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }

        public int TotalCount { get; set; }
    }
}