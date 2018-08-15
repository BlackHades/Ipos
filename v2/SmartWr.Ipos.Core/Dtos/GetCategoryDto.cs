using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Dtos
{
    public class GetCategoryDto : BaseEntity
    {
        public int CategoryUId { get; set; }
        public int ProductCount { get; set; }
        public string Name { get; set; }
        public int? TotalCount { get; set; }
        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }

        public string Description { get; set; }
    }
}