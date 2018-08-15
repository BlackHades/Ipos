
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;


namespace SmartWr.Ipos.Core.Utilities
{
    public static class CurrentTabMenuHelper
    {
        public static string MakeActiveClass(this UrlHelper urlHelper, string controller)
        {               
            string result = "active";

            string controllerName = urlHelper.RequestContext.RouteData.Values["controller"].ToString();

            if (!controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }

            return result;
        }
    }
}
