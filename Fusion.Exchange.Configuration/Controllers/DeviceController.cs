using Fusion.Exchange.Configuration.Contexto;
using Fusion.Framework.Exchange.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fusion.Exchange.Configuration.Controllers
{
    public class DeviceController : ApiController
    {
      
        private ConfiguracaoContexto db = new ConfiguracaoContexto();
        /// <summary>
        /// Obtem a configuração do dispositivo
        /// </summary>
        /// <returns>Dispositivo</returns>
        [HttpGet]
        public Dispositivo GetDevice(string serial)
        {            
            var dispositivo = db.DispositivoModels.Where(x => x.Serial == serial).FirstOrDefault();

            if (dispositivo == null)
            {
                // Inclui o dispositivo no banco para permitir paridade
                dispositivo = new Dispositivo();
                dispositivo.Ativo = true;
                dispositivo.DataAtivacao = DateTime.Now;
                dispositivo.MAC = serial;
                dispositivo.Nome = "Sala de Reunião [" + serial + "]";
                dispositivo.Serial = serial;
                dispositivo.TimeZone = "E. South America Standard Time";
                dispositivo.Token = Guid.NewGuid().ToString();
                dispositivo.CodigoCliente = null;
                dispositivo.Cliente = null;
                db.DispositivoModels.Add(dispositivo);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    string msg = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        msg += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (var ve in eve.ValidationErrors)
                        {
                            msg += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    if (dispositivo != null)
                    {
                        dispositivo.Token = msg;
                    }
                    //throw new Exception("Falha na verificação de dados." + Environment.NewLine + msg);
                }
            }

            var retorno = dispositivo;

            return retorno;
        }

        /// <summary>
        /// Obtem os dispositivos pareados por cliente
        /// </summary>
        /// <returns>Dispositivos</returns>        
        [HttpGet]
        public List<Dispositivo> GetDispositivosPareados()
        {
            var dispositivoModels = db.DispositivoModels.Where(x => x.CodigoCliente != null).ToList(); 

            return dispositivoModels;
        }
    }
}
