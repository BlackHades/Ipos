using System;
using System.Collections.Generic;
using SmartWr.WebFramework.Library.Infrastructure.Validation;
using SmartWr.WebFramework.Library.MiddleServices.Models;

namespace SmartWr.Ipos.Core.Models
{
    public partial class aspnet_SchemaVersions:BaseEntity
    {
        public string Feature { get; set; }
        public string CompatibleSchemaVersion { get; set; }
        public bool IsCurrentVersion { get; set; }

        public override List<ValidationError> Validate()
        {
            throw new NotImplementedException();
        }
    }
}
