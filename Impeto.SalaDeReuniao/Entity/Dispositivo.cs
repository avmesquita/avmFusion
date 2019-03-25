using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Impeto.SalaDeReuniao.Entity
{
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
        [Required(ErrorMessage = "Informe o número de série da Sala de Reunião")]
        public string Serial { get; set; }

        [Display(Name = "Nome")]
        [Column("TXT_NOME")]
        //[Required(ErrorMessage = "Informe o nome da Sala de Reunião")]
        public string Nome { get; set; }

        [Display(Name = "SSID da Rede Wifi")]
        [Column("TXT_SSID_ID")]
        //[Required(ErrorMessage = "Informe o SSID da Rede Wifi")]
        public string SSID_ID { get; set; }

        [Display(Name = "Senha da Rede Wifi")]
        [Column("TXT_SSID_PASS")]
        //[Required(ErrorMessage = "Informe a senha da Rede Wifi")]
        public string SSID_PASS { get; set; }

        [Display(Name = "Endereço MAC do Dispositivo")]
        [Column("TXT_MAC")]
        //[Required(ErrorMessage = "Informe o Endereço MAC da Sala de Reunião")]
        public string MAC { get; set; }

        [Display(Name = "Token de Segurança")]
        [Column("TXT_TOKEN")]
        //[Required(ErrorMessage = "Informe o Token de segurança")]
        public string Token { get; set; }

        [Display(Name = "Tempo Fixo de Atualização")]
        [Column("NUM_TEMPO_FIXO")]
        public int TempoFixo { get; set; }

        [Display(Name = "Tempo de Atualização do Evento")]
        [Column("NUM_TEMPO_EVNT")]
        public int TempoEvento { get; set; }

        [Display(Name = "Data de Ativação da Sala de Reunião")]
        [Column("DAT_ATIVACAO")]
        public DateTime DataAtivacao { get; set; }

        [Display(Name = "Ativo")]
        [Column("IND_ATIVO")]
        public bool Ativo { get; set; }

        [Display(Name = "Código do Cliente")]
        [Column("COD_CLIENTE")]
        public int CodigoCliente { get; set; }

        [ForeignKey("CodigoCliente")]
        public virtual Cliente Cliente { get; set; }


        public Dispositivo()
        {
            Codigo = 0;
            Nome = "";
            SSID_ID = "";
            SSID_PASS = "";
            MAC = "";
            Token = "";
            TempoFixo = 60;
            TempoEvento = 60;
            DataAtivacao = DateTime.Now;
            Ativo = true;            
        }
    }
}