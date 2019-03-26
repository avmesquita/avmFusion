using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
//using System.Web.Mvc;
using Fusion.Exchange.Configuration.Contexto;
using Fusion.Exchange.Configuration.Models;
using System.Net.Http;
using System.Net.Http.Formatting;
using Fusion.Framework.Exchange.Entity;
using System.Data.Entity.Validation;
using System.Web.Http;

namespace Fusion.Exchange.Configuration.Controllers
{
    public class LogEventoController: ApiController
    {
        private ConfiguracaoContexto db = new ConfiguracaoContexto();

        [HttpPost]
        public LogEvento InclusaoLogEvento(int codigo, DateTime dataAtivacao, string token, bool ativo)
        {
            var logEvento = new LogEvento();

            logEvento.CodigoDispositivo = codigo;
            logEvento.Status = ativo;
            logEvento.Token = token;
            logEvento.DataEvento = dataAtivacao;

            db.LogEventoModels.Add(logEvento);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                logEvento.MensagemErro = e.Message;
            }
            return logEvento;
        }

        /// <summary>
        /// Obtem o percentual de dispositivos ativos na mesma data?
        /// </summary>
        /// <returns>Boolean</returns>
        [HttpGet]
        public bool TestarAmostragemDispositivoAtivo(string smtp)
        {
            var dispositivo = db.DispositivoModels.Where(x => x.Smtp == smtp).FirstOrDefault();
            var perc = 0;
            if (dispositivo != null)
            {
                var idDispositivo = dispositivo.Codigo;
                var dataHoje = DateTime.Today;
                // Obtem o total de post de um determinado dispositovo no dia.
                var total = db.LogEventoModels.Where(x => x.CodigoDispositivo == idDispositivo && x.DataEvento > dataHoje.AddHours(-1)).Count();
                // Obtem o total de post de um determinado dispositivo ativo no dia.
                var ativo = db.LogEventoModels.Where(x => x.CodigoDispositivo == idDispositivo && x.Status == true && x.DataEvento > dataHoje.AddHours(-1)).Count();
                // Calcula o percentual de ativos.
                perc = ativo * 100 / total;
            }
            return (perc >= 50);
        }
    }
}