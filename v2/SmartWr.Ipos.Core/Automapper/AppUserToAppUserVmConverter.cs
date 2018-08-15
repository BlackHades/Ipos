using AutoMapper;
using SmartWr.Ipos.Domain.ViewModels;
using SmartWr.WebFramework.Library.MiddleServices.Models.Auth;

namespace SmartWr.Ipos.Core.Automapper
{
    public class AppUserToAppUserVmConverter : ITypeConverter<AppUser, AppUserViewModel>
    {
        public AppUserViewModel Convert(ResolutionContext context)
        {
            var data = context.SourceValue as AppUser;
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
