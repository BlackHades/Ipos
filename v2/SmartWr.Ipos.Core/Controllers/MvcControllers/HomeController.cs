using System.Net;
using System.Web.Mvc;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class HomeController : BaseMvcController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ItemDetailView()
        {
            if (!Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "This resouce cannot be served.");
            return PartialView("~/Views/Home/Partials/_RecentItemDetails.cshtml");
        }

        public ActionResult FaultyItems()
        {
            if (!Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "This resouce cannot be served.");
            return PartialView("~/Views/Home/Partials/_GetFaultyProducts.cshtml");
        }

        public ActionResult ReStockItems()
        {
            if (!Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "This resouce cannot be served.");
            return PartialView("~/Views/Home/Partials/_GetRestockItems.cshtml");
        }

        public ActionResult RecentBoughtItems()
        {
            if (!Request.IsAjaxRequest())
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "This resouce cannot be served.");
            return PartialView("~/Views/Home/Partials/_GetRecentSales.cshtml");
        }
    }
}