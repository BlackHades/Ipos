using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core.Caching;
using SmartWr.Ipos.Core.Settings;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class ToolsController : BaseMvcController
    {
        private ICacheManager _cacheManager;
        private IApplicationUserManager _appUserMgr;
        private readonly string userKey = "ipos_users";

        [HttpGet]
        public ActionResult Main()
        {
            return View("~/views/tools/toolsshell.cshtml");
        }

        public ActionResult WastedItems()
        {
            return View();
        }

        public ActionResult EditWasteItem()
        {
            return View();
        }

        public ActionResult AuditTrail()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult SmsApp()
        {
            return View();
        }

        public ActionResult EmailApp()
        {
            return View();
        }

        public ActionResult ComposeEmail()
        {
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            var users = _cacheManager.Get<IEnumerable<string>>(userKey);

            if (users == null)
            {
                _appUserMgr = EngineContext.Current.Resolve<IApplicationUserManager>();
                ///Todo: Use sql for this
                users = _appUserMgr.GetUsers().Where(user => !user.LockoutEnabled).Select(p => p.Email);
                _cacheManager.Set(userKey, users, AppKeys.DefaultCacheTime);
            }
            ViewBag.users = new SelectList(users);
            return View();
        }

        public ActionResult SentEmails()
        {
            return View();
        }
    }
}