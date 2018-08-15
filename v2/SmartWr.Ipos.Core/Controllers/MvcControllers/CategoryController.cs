using System.Web.Mvc;
using SmartWr.Ipos.Core.Context.Services;
using System.Threading.Tasks;
using Nop.Core.Caching;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using System.Collections.Generic;
using SmartWr.Ipos.Core.Settings;
using System.Linq;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class CategoryController : BaseMvcController
    {
        private readonly CategoryService _catSvc;
        private ICacheManager _cacheManager;
        private string categoryKey = "categoryNames";



        public CategoryController(CategoryService catSvc)
        {
            _catSvc = catSvc;
        }

        [HttpGet]
        public ActionResult CategoryCreate()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CategoryViewDetail()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CategoryEdit()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListCategory()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Main()
        {
            return View("~/views/category/categoryshell.cshtml");
        }

        [HttpGet]
        public JsonResult CheckExistingCategory(string name)
        {
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            var categories = _cacheManager.Get<IEnumerable<string>>(categoryKey);

            if (categories == null)
            {
                categories = _catSvc.GetAllCategories().Select(p => p.Name);
                _cacheManager.Set(categoryKey, categories, AppKeys.DefaultCacheTime);
            }

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

    }
}