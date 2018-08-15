using System.Web.Mvc;
using SmartWr.Ipos.Core.Context.Services;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class StockController : BaseMvcController
    {
        private readonly CategoryService _catSvc;

        public StockController(CategoryService catSvc)
        {
            _catSvc = catSvc;
        }

        public ActionResult StockMgt()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListProducts()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Main()
        {
            return View("~/views/stock/stockshell.cshtml");
        }

        [HttpGet]
        public ActionResult StockCreate()
        {
            PrefillCategories();
            return View();
        }

        [HttpGet]
        public ActionResult StockEdit()
        {
            PrefillCategories();
            return View();
        }

        public ActionResult StockViewDetail()
        {
            PrefillCategories();
            return View();
        }

        private void PrefillCategories() {
            ViewBag.Category = new SelectList(_catSvc.GetCategories(), "CategoryUId", "Name");
        }
    }
}