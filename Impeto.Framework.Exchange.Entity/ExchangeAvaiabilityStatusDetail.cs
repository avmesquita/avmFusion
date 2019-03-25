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
    [Table("TB_EXCHANGE_AVAIABILITY_STATUS_DETAIL")]
    public class ExchangeAvaiabilityStatusDetail
    {
        [Key]
        [Column("COD_EXCHANGE_AVAIABILITY_STATUS_DETAIL")]
        public int CodigoAvaiabilityStatusDetail { get; set; }

        [Column("DAT_INICIO")]
        public DateTime? DataInicio { get;set;}

        [Column("DAT_FIM")]
        public DateTime? DataFim { get; set; }

        [Column("COD_STATUS")]
        public int CodigoStatusOcupacao { get; set; }

        [Column("IND_IS_EXCEPTION")]
        public bool IsException { get; set; }

        [Column("IND_IS_MEETING")]
        public bool IsMeeting { get; set; }

        [Column("IND_IS_PRIVATIVE")]
        public bool IsPrivative { get; set; }

        [Column("IND_IS_REMINDER_SET")]
        public bool IsReminderSet { get; set; }

        [Column("TXT_LOCATION")]
        public string Location { get; set; }

        [Column("TXT_STORE_ID")]
        public string StoreId { get; set; }

        [Column("IND_SUBJECT")]
        public string Subject { get; set; }

        [Column("COD_EXCHANGE_AVAIABILITY_STATUS")]
        public int CodigoExchangeAvaiabilityStatus { get; set; }

        [ForeignKey("CodigoExchangeAvaiabilityStatus")]
        public virtual ExchangeAvaiabilityStatus ExchangeAvaiabilityStatus { get; set; }

        public ExchangeAvaiabilityStatusDetail()
        {
            this.CodigoAvaiabilityStatusDetail = 0;
            this.CodigoStatusOcupacao = -1;
            this.DataFim = null;
            this.DataInicio = null;
            this.IsException = false;
            this.IsMeeting = false;
            this.IsPrivative = false;
            this.IsReminderSet = false;
            this.Location = "";
            this.StoreId = "";
            this.Subject = "";
         
        }
    }
}
