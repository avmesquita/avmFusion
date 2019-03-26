using System.Web;
using System.Web.Mvc;

namespace Fusion.Exchange.Configuration
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
