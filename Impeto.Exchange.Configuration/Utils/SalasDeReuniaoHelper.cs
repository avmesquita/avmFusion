using Impeto.Exchange.Configuration.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Impeto.Exchange.Configuration.Utils
{
    public class SalasDeReuniaoHelper
    {
        public List<ListItem> SalasDeReuniao { get; set; }

        public SalasDeReuniaoHelper()
        {
            
        }

        public SalasDeReuniaoHelper(int? id)
        {
            SalasDeReuniao = ObterSalasDoCliente(id);
        }

        private ConfiguracaoContexto db = new ConfiguracaoContexto();

        public List<ListItem> ObterSalasDoCliente(int? id)
        {
            if (id == null)
            {
                return null;
            }
            int codigoCliente = Convert.ToInt32(id);

            var cliente = db.ClienteModels.Where(t => t.Codigo == codigoCliente).FirstOrDefault();

            string email = cliente.Smtp;
            string senha = cliente.Senha;

            Configuration.svcExchange.ImpetoExchangeServiceClient client = new Configuration.svcExchange.ImpetoExchangeServiceClient();

            #region Obter Disponibilidade
            var disponibilidade = client.obterDisponibilidadeTimeZoneFull(email, senha, "E. South America Standard Time");
            #endregion

            if (disponibilidade.Count() > 0)
            {
                var retorno = (from d in disponibilidade
                               select new ListItem
                               {
                                   Text = d.Smtp,
                                   Value = d.Smtp,
                                   Selected = false,
                                   Enabled = true
                               }).ToList();

                return retorno;
            }

            return null;

        }


    }
}