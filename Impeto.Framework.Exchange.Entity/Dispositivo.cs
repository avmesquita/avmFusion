using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Impeto.Framework.Exchange.Entity
{
    [Serializable]
    [Table("TB_DISPOSITIVO")]
    public class Dispositivo
    {
        [Key]
        [Display(Name = "Código")]
        [Column("COD_DISPOSITIVO")]
        public int Codigo { get; set; }

        [Display(Name = "Serial")]        
        [Index(IsUnique = true)]
        [Column("TXT_SERIAL")]
        [MaxLength(12)]
        [Required(ErrorMessage = "Informe o número de série da Sala de Reunião")]
        public string Serial { get; set; }

        [Display(Name = "Nome")]
        [Column("TXT_NOME")]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Display(Name = "Smtp")]
        [Column("TXT_SMTP")]
        [MaxLength(100)]
        public string Smtp { get; set; }



        [Display(Name = "Endereço MAC do Dispositivo")]
        [Column("TXT_MAC")]
        [MaxLength(12)]
        //[Required(ErrorMessage = "Informe o Endereço MAC da Sala de Reunião")]
        public string MAC { get; set; }

        [Display(Name = "Token de Segurança")]
        [Column("TXT_TOKEN")]
        [MaxLength(100)]
        //[Required(ErrorMessage = "Informe o Token de segurança")]
        public string Token { get; set; }

        [Display(Name = "Data de Ativação da Sala de Reunião")]
        [Column("DAT_ATIVACAO")]
        public DateTime DataAtivacao { get; set; }

        [Display(Name = "Ativo")]
        [Column("IND_ATIVO")]
        public bool Ativo { get; set; }

        [Display(Name = "Código do Cliente")]
        [Column("COD_CLIENTE")]
        public int? CodigoCliente { get; set; }

        [ForeignKey("CodigoCliente")]
        public virtual Cliente Cliente { get; set; }

        [Display(Name = "Fuso Horário")]
        [Column("TXT_TIMEZONE")]
        [MaxLength(200)]
        [Required(ErrorMessage = "Selecione o Fuso Horário")]
        public string TimeZone { get; set; }

        public Dispositivo()
        {
            Codigo = 0;
            Nome = "";
            MAC = "";
            Token = ""; //Guid.NewGuid().ToString() // Se gerar o grid aqui atrapalha a troca de mensagens
            TimeZone = "E. South America Standard Time";
            DataAtivacao = DateTime.Now;
            Ativo = true;
            CodigoCliente = null;
            Cliente = null;
            Smtp = "";
        }
    }
}