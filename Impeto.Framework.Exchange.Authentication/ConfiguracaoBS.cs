using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Authentication
{
    public class ConfiguracaoBS
    {
        public Configuracao obterConfiguracao()
        {
            Configuracao retorno = new Configuracao();

            if (ConfigurationManager.AppSettings["EmailAddress"] != null)
            {
                retorno.EmailAddress = ConfigurationManager.AppSettings["EmailAddress"];
            }
             
            if (ConfigurationManager.AppSettings["Password"] != null)
            {
                retorno.Password = ConfigurationManager.AppSettings["Password"];
            }

            if (ConfigurationManager.AppSettings["AutoDiscover"] != null)
            {
                retorno.AutoDiscover = ConfigurationManager.AppSettings["AutoDiscover"];
            }

            if (ConfigurationManager.AppSettings["SalasEstaticas"] != null)
            {
                retorno.SalasEstaticas = ConfigurationManager.AppSettings["SalasEstaticas"];
            }

            return retorno;        
        }
    }
}
