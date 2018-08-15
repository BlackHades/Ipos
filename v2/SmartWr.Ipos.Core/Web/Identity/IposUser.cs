using System;
using System.Security.Claims;
using SmartWr.WebFramework.Library.Infrastructure.Auth;

namespace SmartWr.Ipos.Core.Web.Identity
{
    public class IposUser: CustomAppUser
    {
         public IposUser(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public new Int32 Id 
        { 
            get
            {
                return Int32.Parse((base.Id));
            }
        }

    }
}
