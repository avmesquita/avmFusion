using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Authentication
{
    public class Configuracao
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string AutoDiscover { get; set; }
        public string ArduinoService { get; set; }
        public string SalasEstaticas { get; set; }

        public Configuracao()
        {
            this.EmailAddress = string.Empty;
            this.Password = string.Empty;
            this.AutoDiscover = string.Empty;
            this.ArduinoService = string.Empty;
            this.SalasEstaticas = string.Empty;
        }
    }
}
