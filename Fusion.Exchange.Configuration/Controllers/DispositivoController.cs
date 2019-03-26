using Fusion.Exchange.Configuration.Contexto;
using Fusion.Framework.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;


namespace Fusion.Exchange.Configuration.Controllers
{
    public class DispositivoController : Controller
    {
        private ConfiguracaoContexto db = new ConfiguracaoContexto();
        /*       
        /// <summary>
        /// Obtem a configuração do dispositivo
        /// </summary>
        /// <returns>Dispositivo</returns>
        //[HttpGet]
        public Dispositivo GetDispositivo(string serial)
        {            
            var dispositivoModels = db.DispositivoModels.Where(x => x.Serial == serial);

            var retorno = dispositivoModels.FirstOrDefault();

            return retorno;
        }
        */

        
        public async Task<ActionResult> Todos()
        {
            try
            {
                var dispositivoModels = db.DispositivoModels.ToListAsync();
                return View(await dispositivoModels);
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message.ToString();
                //throw ex;
            }
            return View();
        }

        public async Task<ActionResult> NaoPareados()
        {
            try
            {
                var dispositivoModels = db.DispositivoModels.Where(t => t.CodigoCliente == null).ToListAsync();
                return View(await dispositivoModels);
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message.ToString();
            }
            return View();

        }

        public ActionResult ParearNovo()
        {

            return View();
        }
        
        [System.Web.Http.HttpPost]        
        public ActionResult Verificar(string serial)
        {
            bool retorno = false;
            string dataAtualizacao = "";
            bool temGente = false;

            if (!string.IsNullOrEmpty(serial))
            {                
                var sala = GetSala(serial);

                if ((sala.Result != null) && (sala.Result.Smtp != string.Empty))
                {
                    retorno = true;
                    dataAtualizacao = sala.Result.DataAtualizacao.ToString();
                    temGente = sala.Result.HasPeople;
                }

                if (!retorno)
                {
                    var dispositivo = db.DispositivoModels
                                        .Include(x => x.Cliente)
                                        .Where(x => x.Serial == serial && x.Cliente == null)
                                        .FirstOrDefault();

                    if (dispositivo != null)
                    {
                        retorno = true;
                        dataAtualizacao = "";
                        temGente = false;
                    } else
                    {
                        retorno = false;
                        dataAtualizacao = "";
                        temGente = false;
                    }
                }

            }

            return new JsonResult { Data = new { status = retorno, dataAtualizacao = dataAtualizacao, temGente = temGente } };
        }
    
        public async Task<Framework.Exchange.Models.SalaDeReuniao> GetSala(string serial)
        {
            const string devPath = "http://localhost:59138/Fusion.Exchange.Arduino/Fusion.Exchange.Arduino/api/Arduino";
            const string prodPath = "http://Fusionexchangearduino.azurewebsites.net/Fusion.Exchange.Arduino/api/Arduino";

            string path = "";
            string param = "/GetDevice?serial=" + serial;

            //if (Debugger.IsAttached)
            //{
            //    path = devPath + param;
            //}
            //else
            //{
                path = prodPath + param;
            //}
           
            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;
            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            try
            {
                var lista = y.Content.ReadAsAsync<Framework.Exchange.Models.SalaDeReuniao>(formatters);
                return await lista;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
