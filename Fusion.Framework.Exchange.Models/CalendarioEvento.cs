using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Framework.Exchange.Models
{
    [Serializable]
    public class CalendarioEvento
    {
        // Summary:
        //     Gets the details of the calendar event. Details is null if the user requsting
        //     them does no have the appropriate rights.
        public CalendarioEventoDetalhe Details { get; set; }
        //
        // Summary:
        //     Gets the end date and time of the event.
        public DateTime EndTime { get; set; }
        //
        // Summary:
        //     Gets the free/busy status associated with the event.
        public StatusLegadoLivreOcupado FreeBusyStatus { get; set; }
        //
        // Summary:
        //     Gets the start date and time of the event.
        public DateTime StartTime { get; set; }
    }
}
