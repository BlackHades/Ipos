using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Core.Enums;
using SmartWr.Ipos.Core.Utilities;
using SmartWr.Ipos.Core.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class AuditDtoVmConverter : ITypeConverter<AuditDto, AuditViewModel>
    {
        public AuditViewModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as AuditDto;

            return new AuditViewModel
            {
                AuditId = data.AuditId,
                AuditType = data.AuditType.HasValue ? Extension.GetEnumDescription(typeof(AuditType), data.AuditType) : "NA",
                Description = data.Description ?? "NA",
                EntryDate = data.EntryDate.HasValue ? data.EntryDate.Value.ToString("ddd, dd MMM yyyy") : "NA",
                UserName = data.UserName ?? "NA"
            };
        }
    }
}