using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Nop.Core.Caching;
using SmartWr.Ipos.Core.Settings;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using SmartWr.Ipos.Core.Context.Services;
using SmartWr.Ipos.Core.Models;
using SmartWr.Ipos.Core.Dtos;
using System;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class TransactionController : BaseMvcController
    {
        private readonly string userKey = "ipos_users";
        private readonly string stockKey = "ipos_stocks";
        private ICacheManager _cacheManager;
        private readonly ProductService _prodSvc;

        public TransactionController(ProductService prodSvc)
        {
            _prodSvc = prodSvc;
        }

        public ActionResult Main()
        {
            return View("~/views/transaction/transactionshell.cshtml");
        }

        public ActionResult SalesHistory()
        {
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            var users = _cacheManager.Get<IEnumerable<string>>(userKey);

            if (users == null)
            {
                users = Membership.GetAllUsers().Cast<MembershipUser>().Select(p => p.UserName);
                _cacheManager.Set(userKey, users, AppKeys.DefaultCacheTime);
            }

            var stock = _cacheManager.Get<IEnumerable<ProductDto>>(stockKey);

            if (stock == null)
            {
                stock = _prodSvc.GetPagedProducts(1, 10000, null);
                _cacheManager.Set(stockKey, stock, AppKeys.DefaultCacheTime);
            }

            ViewBag.user = new SelectList(users);
            ViewBag.Stock = new SelectList(stock, "ProductId", "FullDescription");
            return View();
        }

        public ActionResult TransactionDetails()
        {
            return View();
        }

        public ActionResult PendingPost()
        {
            return View();
        }
    }
}