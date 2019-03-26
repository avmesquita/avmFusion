using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusion.Framework.Exchange.Entity
{
    [Table("TB_EXCHANGE_AVAIABILITY_EMAIL")]
    public class ExchangeAvaiabilityEmail
    {
        [Key]
        [Column("COD_EXCHANGE_AVAIABILITY_EMAIL")]
        public int CodigoExchangeAvaiabilityEmail { get; set; }

        [Column("TXT_SMTP")]
        public string Smtp { get; set; }

        [Column("TXT_STORE_ID")]
        public string StoreId { get; set; }

        [Column("IND_STATUS")]
        public string StatusEnvio { get; set; }

        [Column("DAT_ENVIO")]
        public DateTime DataEnvio { get; set; }

        public ExchangeAvaiabilityEmail()
        {
            this.CodigoExchangeAvaiabilityEmail = 0;
            this.DataEnvio = DateTime.Now;
            this.Smtp = "";
            this.StatusEnvio = "N";
            this.StoreId = "";
        }

        public ExchangeAvaiabilityEmail(string smtp, string storeid, string statusenvio)
        {            
            this.CodigoExchangeAvaiabilityEmail = 0;
            this.DataEnvio = DateTime.Now;
            this.Smtp = smtp;
            this.StatusEnvio = statusenvio;
            this.StoreId = storeid;
        }


    }
}
