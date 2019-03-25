using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]
    public class Status
    {        
        public StatusDisponibilidade StatusDisponibilidade { get; set; }        
        public DateTime? SugestaoProximaReuniao { get; set; }
        public string Mensagem { get; set; }
        public List<CalendarioEvento> ListaEventos { get; set; }

        public Status()
        {
            this.StatusDisponibilidade = Models.StatusDisponibilidade.Desconhecido;
            this.SugestaoProximaReuniao = null;
            this.Mensagem = "";
            this.ListaEventos = new List<CalendarioEvento>();
        }
    }
}
