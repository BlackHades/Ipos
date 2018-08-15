using System.Web.Mvc;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class ErrorController : BaseMvcController
    {
        public ActionResult NotFound()
        {
            if (Request.IsAjaxRequest())
                return PartialView("~/views/error/_notfoundpartial.cshtml");
            return View("~/views/error/notfound.cshtml");
        }

        public ActionResult ServerError()
        {
            if (Request.IsAjaxRequest())
                return PartialView("~/views/error/_servererrorpartial.cshtml");
            return View("~/views/error/servererror.cshtml");
        }
    }
}