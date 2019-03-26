using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Fusion.Framework.Exchange.Entity
{
    [Serializable]
    [Table("TB_LOG_EVENTO")]
    public class LogEvento
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Display(Name = "Sequencial")]
        [Column("NUM_SEQUENCIAL_LOG_EVENTO")]
        public int CodigoLogEvento { get; set; }

        [Display(Name = "Código")]
        [Column("COD_DISPOSITIVO")]
        public int CodigoDispositivo { get; set; }

        [Display(Name = "Data Evento")]
        [Column("DAT_EVENTO")]
        public DateTime DataEvento { get; set; }

        [Display(Name = "Token")]
        [Column("TXT_TOKEN")]
        public string Token { get; set; }

        [Display(Name = "Status")]
        [Column("IND_STATUS")]
        public bool Status { get; set; }

        [Display(Name = "Mensagem")]
        public virtual string MensagemErro { get; set; }

        public LogEvento()
        {
            DataEvento = DateTime.Now;
        }
    }
}
