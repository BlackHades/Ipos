using System.Web.Mvc;

namespace SmartWr.Ipos.Core.Web.App_Start
{
    public class FilterConfig
    {
        public static void Filter(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
        }
    }
}
