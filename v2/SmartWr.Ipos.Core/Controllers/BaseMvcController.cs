using System.Security.Claims;
using System.Web.Mvc;
using SmartWr.Ipos.Core.Web.Identity;
using SmartWr.Ipos.Core.Web.Mvc.Filters;

namespace SmartWr.Ipos.Core.Controllers
{
    [NoCache]
    public abstract class BaseMvcController : Controller
    {
        public IposUser CurrentUser
        {
            get
            {
                return new IposUser(this.User as ClaimsPrincipal);
            }
        }
    }
}