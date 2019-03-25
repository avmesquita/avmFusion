using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]
    public class CalendarioEventoDetalhe
    {
        // Summary:
        //     Gets a value indicating whether the calendar event is an exception in a recurring
        //     series.
        public bool IsException { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the calendar event is a meeting.
        public bool IsMeeting { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the calendar event is private.
        public bool IsPrivate { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the calendar event is recurring.
        public bool IsRecurring { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the calendar event has a reminder set.
        public bool IsReminderSet { get; set; }
        //
        // Summary:
        //     Gets the location of the calendar event.
        public string Location { get; set; }
        //
        // Summary:
        //     Gets the store Id of the calendar event.
        public string StoreId { get; set; }
        //
        // Summary:
        //     Gets the subject of the calendar event.
        public string Subject { get; set; }
    }
}
