using Fusion.Framework.Exchange.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Fusion.Exchange.Arduino.Controllers
{
    public class ArduinoController : ApiController
    {
        /// <summary>
        /// Obtem todos os Estados de todas as Salas de Reuniões
        /// </summary>
        /// <returns>Lista de Salas de Reunião</returns>
        [HttpGet]
        public List<SalaDeReuniao> Get()
        {
            var lista = ModeloDeEstado.Instance.ListaDeModelosDeEstados;

            return lista;
        }

        [HttpGet]
        public SalaDeReuniao GetDevice(string serial)
        {
            var device = ModeloDeEstado.Instance.ListaDeModelosDeEstados.Where(x => x.Smtp == serial).FirstOrDefault();

            return device;
        }

        /// <summary>
        /// Obtem todos os Estados de todas as Salas de Reuniões
        /// </summary>
        /// <returns>Lista de Salas de Reunião</returns>
        [HttpGet]
        public List<SalaDeReuniao> GetHistorico()
        {
            var lista = ModeloDeEstado.Instance.ListaDeHistorico;

            return lista;
        }

        /// <summary>
        /// Inclui ou Atualiza um endereço smtp e sua disponibilidade IoT
        /// </summary>
        /// <param name="smtp">Identificador para busca de Status no Exchange</param>
        /// <param name="haspeople">Indicador de presença na sala de reunião</param>
        /// <returns>OK ou BAD</returns>
        [HttpPost]
        //[Route("Fusion.Exchange.Arduino/api/Arduino/Post")]
        [Route("Fusion.Exchange.Arduino/api/Arduino/Post/{smtp}/{haspeople}")]
        public string Post(string smtp, string haspeople)//, int idCliente = 0)
        {
            var retorno = ModeloDeEstado.Instance.SaveOrUpdate(smtp, haspeople);

            return retorno;
        }

        /// <summary>
        /// Limpa todos os ítens do Modelo de Estado
        /// </summary>
        /// <param name="token">Token de segurança</param>
        /// <returns>OK or BAD</returns>        
        [HttpPost]
        [Route("Fusion.Exchange.Arduino/api/Arduino/Limpar/{token}")]
        public string Limpar(string token)
        {
            string retorno = "BAD";
            if (token != null && token.ToLower().Contains("cid"))
            {
                retorno = ModeloDeEstado.Instance.Limpar();
                return retorno;
            }
            return retorno;
        }        
        /// <summary>
        /// Limpa todos os ítens do Historico
        /// </summary>
        /// <param name="token">Token de segurança</param>
        /// <returns>OK or BAD</returns>
        /*
        [HttpPost]
        [Route("Fusion.Exchange.Arduino/api/Arduino/LimparHistorico/{token}")]
        public string LimparHistorico(string token)
        {
            string retorno = "BAD";
            if (token != null && token.ToLower().Contains("cid"))
            {
                retorno = ModeloDeEstado.Instance.LimparHistorico();
                return retorno;
            }
            return retorno;
        }
        
        public async Task<IEnumerable<Dispositivo>> GetConfiguration(string serial)        
        {
            string path = string.Format("http://Fusionexchangeconfiguration.azurewebsites.net/api/Dispositivo/GetDevice/{0}",serial);

            HttpClient cliente = new HttpClient();

            var x = cliente.GetAsync(path);
            var y = x.Result;

            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            var dispositivo = y.Content.ReadAsAsync<IEnumerable<Dispositivo>>(formatters);

            return await dispositivo;

        }
        */
    }
}
