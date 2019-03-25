using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Impeto.Exchange.Portal.Controllers
{
    public class GestaoController : Controller
    {
        [Authorize]
        public ActionResult MinhaConta()
        {
            return View();
        }

        [Authorize]
        public ActionResult StatusDispositivos()
        {
            return View();
        }

        [Authorize]
        public ActionResult StatusSalas()
        {
            return View();
        }



    }
}