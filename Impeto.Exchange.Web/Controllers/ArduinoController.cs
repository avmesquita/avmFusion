using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Impeto.Exchange.Web.Controllers
{
    public class ArduinoController : Controller
    {
        // GET: Arduino        
        public ActionResult Index()
        {            
            var salasDeReuniao = this.GetSalas().Result;

            string header = "<h3>Status do Dispositivo IoT</h3>";

            string listagem = "<table border='1' cellspacing='3' cellspading='3' width='90%'>";

            listagem += "<tr>";
            listagem += "<td>Smtp do O365</td>";
            listagem += "<td>Cód.Cliente</td>";
            listagem += "<td>Status do Sensor</td>";
            listagem += "<td>Update DT</td>";
            listagem += "</tr>";

            foreach (var sala in salasDeReuniao)
            {
                listagem += "<tr>";
                listagem += "<td>"+ sala.Smtp +"</td>";
                listagem += "<td>" + sala.IdCliente.ToString() + "</td>";
                listagem += "<td>"+Convert.ToString(sala.HasPeople)+"</td>";
                listagem += "<td>"+sala.DataAtualizacao.ToString()+"</td>";
                listagem += "</tr>";           
            }
            listagem += "</table>";

            // Historico
            var salasHistorico = this.GetHistorico().Result;

            string headerHistorico = "<h3>Últimas 10 atualizações</h3>";

            string listagemHistorico = "<table border='1' cellspacing='3' cellspading='3' width='90%'>";

            listagemHistorico += "<tr>";
            listagemHistorico += "<td>Smtp do O365</td>";
            listagemHistorico += "<td>Cód.Cliente</td>";
            listagemHistorico += "<td>Status do Sensor</td>";
            listagemHistorico += "<td>Update DT</td>";
            listagemHistorico += "</tr>";

            foreach (var sala in salasHistorico)
            {
                listagemHistorico += "<tr>";
                listagemHistorico += "<td>" + sala.Smtp + "</td>";
                listagemHistorico += "<td>" + sala.IdCliente.ToString() + "</td>";
                listagemHistorico += "<td>" + Convert.ToString(sala.HasPeople) + "</td>";
                listagemHistorico += "<td>" + sala.DataAtualizacao.ToString() + "</td>";
                listagemHistorico += "</tr>";
            }
            listagemHistorico += "</table>";

            ViewBag.Listagem = header + listagem;

            ViewBag.ListagemHistorico = headerHistorico + listagemHistorico;

            //ViewBag.MetaTags = "<meta http-equiv='refresh' content='5' >";

            return View();
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

        public async Task<IEnumerable<ArduinoAPI.Models.SalaDeReuniao>> GetHistorico()
        {
            string path = "http://impetoexchangearduino.azurewebsites.net/Impeto.Exchange.Arduino/api/Arduino/GetHistorico";

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