using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fusion.Exchange.Web.Controllers
{
    [HandleError(ExceptionType = typeof(HttpAntiForgeryException),
                 View = "ErrorMessages/NoCookieSupport")]
    [HandleError(View = "ErrorMessages/Default")]
    public class BaseController : Controller
    {
    }
}