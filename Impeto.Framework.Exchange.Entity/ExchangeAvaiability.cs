using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Entity
{
    [Serializable]
    [Table("TB_EXCHANGE_AVAIABILITY")]
    public class ExchangeAvaiability
    {

        [Key]
        [Column("COD_EXCHANGE_AVAIABILITY")]
        public int CodigoExchangeAvaiability { get; set; }

        [Column("TXT_NAME")]
        public string Nome { get; set; }

        [Column("TIP_MEETING_ATTENDEE")]
        public int TipoMeetingAtendee { get; set; }

        [Column("TXT_MESSAGE")]
        public string Mensagem { get; set; }

        public virtual IList<ExchangeAvaiabilityStatus> Status { get; set; }

        public ExchangeAvaiability()
        {
            CodigoExchangeAvaiability = 0;
            Nome = "";
            TipoMeetingAtendee = -1;
            Mensagem = "";

            Status = new List<ExchangeAvaiabilityStatus>();
        }
    }
}
