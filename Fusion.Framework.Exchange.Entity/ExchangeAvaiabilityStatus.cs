using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Framework.Exchange.Entity
{
    [Table("TB_EXCHANGE_AVAIABILITY_STATUS")]
    public class ExchangeAvaiabilityStatus
    {
        [Key]
        [Column("COD_EXCHANGE_AVAIABILITY_STATUS")]
        public int CodigoExchangeAvaibilityStatus { get; set; }


        [Column("COD_EXCHANGE_AVAIABILITY")]
        public int CodigoExchangeAvaiability { get; set; }

        [ForeignKey("CodigoExchangeAvaiability")]
        public ExchangeAvaiability ExchangeAvaiability { get; set; }

        [Column("COD_STATUS_AVAIABILITY")]
        public int CodigoExchangeStatusAvaiability { get; set; }

        [Column("DAT_NEXT_MEETING_ROOM_SUG")]
        public DateTime? NextMeetingRoomSugestion { get; set; }

        [Column("TXT_MESSAGE")]
        public string Message { get; set; }

        public virtual IList<ExchangeAvaiabilityStatusDetail> Detalhes { get; set; }

        public ExchangeAvaiabilityStatus()
        {
            this.CodigoExchangeAvaiability = 0;
            this.CodigoExchangeAvaibilityStatus = 0;
            this.Message = "";
            this.CodigoExchangeStatusAvaiability = -1;
            this.NextMeetingRoomSugestion = null;

            Detalhes = new List<ExchangeAvaiabilityStatusDetail>();
        }


    }
}
