using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class Customer:BaseEntity
    {
        public Customer()
        {
            this.Orders = new List<Order>();
        }

        public int CustomerId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public Nullable<DateTime> EntryDate { get; set; }
        public string Address { get; set; }
        public string Remarks { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
