using AutoMapper;
using SmartWr.Ipos.Core.Dtos;
using SmartWr.Ipos.Domain.ViewModels;

namespace SmartWr.Ipos.Core.Automapper
{
    public class AccountDtoToAppUserVmConverter : ITypeConverter<AccountDto, AppUserViewModel>
    {
        public AppUserViewModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as AccountDto;
            return new AppUserViewModel
            {
                Id = data.Id,
                Email = data.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = data.UserName,
                PhoneNumber = data.PhoneNumber,
                Status = !data.LockoutEnabled
            };
        }
    }
}