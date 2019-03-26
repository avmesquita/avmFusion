using Fusion.Exchange.Web.Contexto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fusion.Framework.Exchange.Entity;
using Fusion.Framework.Exchange.Models;
using System.Data.Entity;
using Fusion.Framework.Exchange.Service;
using Fusion.Exchange.Web.svcExchange;
using System.Net.Http;
using System.Net.Http.Formatting;


namespace Fusion.Exchange.Web.Controllers
{
    public class Office365Controller : Controller
    {
        // GET: Office365        
        public ActionResult Index()
        {
            StringBuilder mensagem = new StringBuilder();

            SalaDeReuniaoContexto db = new SalaDeReuniaoContexto();

            var salas = db.DispositivoModels
                          .Include(x => x.Cliente)
                          .Where(x => x.Ativo == true && x.CodigoCliente != null && x.Smtp != "")
                          .ToList();

            string output = "<table>";

            if (salas.Count > 0)
            {

                foreach (var sala in salas)
                {
                    string storeId = "";
                    string smtp = "";

                    var client = new svcExchange.FusionExchangeServiceClient();

                    var disponibilidadeSalaDispositivo = client.obterDisponibilidadeExchange(sala.Cliente.Smtp,
                                                                                             sala.Cliente.Senha,
                                                                                             sala.Smtp,
                                                                                             sala.TimeZone);

                    output += "<tr><td>" + sala.Smtp.ToString() + " ==> " + sala.Serial.ToString() + " ==> " + disponibilidadeSalaDispositivo.StatusDisponibilidade.ToString() + "</td></tr>";
                }
            }
            output += "</table>";

            ViewBag.Output = output;

            return View();

        }

    }
}