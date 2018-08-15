using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class AuditDto : BaseEntity
    {
        public Guid? AuditId { get; set; }
        public string Description { get; set; }
        public int? TotalCount { get; set; }
        public string UserName { get; set; }
        public int? AuditType { get; set; }
        public DateTime? EntryDate { get; set; }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}