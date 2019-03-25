using Impeto.Exchange.Web.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.IO;
using Impeto.Framework.Exchange.Entity;
using Impeto.Framework.Exchange.Models;
using System.Data.Entity;
using Impeto.Framework.Exchange.Service;
using Impeto.Exchange.Web.svcExchange;

namespace Impeto.Exchange.Web.Controllers
{
    public class HomeController : Controller
    {
        //[Impeto.Exchange.Web.ActionFilters.WhitespaceFilter]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult GetO365()
        {

            StringBuilder mensagem = new StringBuilder();

            SalaDeReuniaoContexto db = new SalaDeReuniaoContexto();

            var salas = db.DispositivoModels
                          .Include(x => x.Cliente)
                          .Where(x => x.Ativo == true && x.CodigoCliente != null && x.Smtp != "")
                          .ToList();

            string output = "<table border='1' cellspacing='3' cellspading='3' width='90%'>";
            output += "<tr><th>SMTP =>> Número de Série ==> Status Exchange</th></tr>";

            if (salas.Count > 0)
            {

                foreach (var sala in salas)
                {
                    var client = new svcExchange.ImpetoExchangeServiceClient();

                    var disponibilidadeSalaDispositivo = client.obterDisponibilidadeExchange(sala.Cliente.Smtp,
                                                                                             sala.Cliente.Senha,
                                                                                             sala.Smtp,
                                                                                             sala.TimeZone);

                    output += "<tr><td>" + sala.Smtp.ToString() + " ==> " + sala.Serial.ToString() + " ==> " + disponibilidadeSalaDispositivo.StatusDisponibilidade.ToString() + "</td></tr>";
                }
            }
            output += "</table>";

            ViewBag.Output = output;

            return PartialView();
        }

        public ActionResult GetArduino()
        {
            var salasDeReuniao = this.GetSalas().Result;

            string listagem = "<table border='1' cellspacing='3' cellspading='3' width='90%'>";
            listagem += "<tr>";
            listagem += "<th>SMTP</th>";
            listagem += "<th>Sensor</th>";
            listagem += "</tr>";

            foreach (var sala in salasDeReuniao)
            {
                listagem += "<tr>";
                listagem += "<td>" + sala.Smtp + "</td>";
                listagem += "<td>" + Convert.ToString(sala.HasPeople) + "</td>";
                listagem += "</tr>";
            }
            listagem += "</table>";

            ViewBag.Arduino = listagem;

            return PartialView();
        }

        public async Task<IEnumerable<ArduinoAPI.Models.SalaDeReuniao>> GetSalas()
        {
            string path = "http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/Arduino/Get";

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {                
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var lista = y.Content.ReadAsAsync<IEnumerable<ArduinoAPI.Models.SalaDeReuniao>>(formatters);

            return await lista;

        }
    }
}
