using System;

namespace SmartWr.Ipos.Core.ViewModels
{
    public class AuditViewModel
    {
        public Guid? AuditId { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string EntryDate { get; set; }
        public string AuditType { get; set; }
    }
}