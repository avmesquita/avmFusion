using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]    
    public class ResultadoDisponibilidadeSalaReuniao
    {                
        public string Nome { get; set; }
        public string Smtp { get; set; }
        public MeetingAttendeeType Tipo { get; set; }
        public Status Status { get; set; }
        public string Mensagem { get; set; }

        public ResultadoDisponibilidadeSalaReuniao()
        {
            this.Nome = String.Empty;
            this.Smtp = String.Empty;
            this.Tipo = MeetingAttendeeType.Room;
            this.Status = new Status();
            this.Mensagem = "";
        }
    }
}
