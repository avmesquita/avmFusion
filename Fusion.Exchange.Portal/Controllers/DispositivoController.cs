using Impeto.Exchange.Portal.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Impeto.Exchange.Portal.Controllers
{
    public class DispositivoController : Controller
    {
        private SalaDeReuniaoContexto db = new SalaDeReuniaoContexto();
        
        public ActionResult Index()
        {
            return View();
        }

        /*
        public async Task<ActionResult> Verificar(string serial)
        {
            bool retorno = false;
            string dataAtualizacao = "";
            bool temGente = false;

            if (!string.IsNullOrEmpty(serial))
            {
                var dispositivo = db.DispositivoModels.Where(x => x.Serial == serial).FirstOrDefault();
                if (dispositivo != null)
                {
                    var sala = await GetSala(serial);
                    retorno = sala != null;
                    dataAtualizacao = sala.DataAtualizacao.ToString();
                    temGente = sala.HasPeople;
                }
            }
            return new JsonResult { Data = new { status = retorno, dataAtualizacao = dataAtualizacao, temGente = temGente } };
        }


        private async Task<SalaDeReuniao> GetSala(string serial)
        {
            string path = "http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/Arduino/GetDevice" + "/" + serial;

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var lista = y.Content.ReadAsAsync<SalaDeReuniao>(formatters);

            return await lista;

        }
        */

    }
}