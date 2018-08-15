using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Spoil:BaseEntity
    {
        public Guid SpoilId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> Product_Id { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<Guid> User_Id { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
