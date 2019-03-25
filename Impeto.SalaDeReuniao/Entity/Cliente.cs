using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Impeto.SalaDeReuniao.Entity
{
    [Table("TB_CLIENTE")]
    public class Cliente
    {
        [Key]
        [Display(Name = "Código")]
        [Column("COD_CLIENTE")]
        public int Codigo { get; set; }

        [Display(Name = "Nome")]
        [Column("TXT_NOME")]
        [Required(ErrorMessage = "Informe o nome do cliente")]
        public string Nome { get; set; }

        [Display(Name = "Conta SMTP")]
        [Column("TXT_SMTP")]
        [Required(ErrorMessage = "Informe o SMTP do cliente")]
        public string Smtp { get; set; }

        [Display(Name = "Senha do SMTP")]
        [Column("TXT_SENHA")]
        [Required(ErrorMessage = "Informe a senha do SMTP do cliente")]
        public string Senha { get; set; }

        [Display(Name = "Fuso Horário")]
        [Column("TXT_TIMEZONE")]
        [Required(ErrorMessage = "Selecione o Fuso Horário")]
        public string TimeZone { get; set; }

        [Display(Name = "Token de Segurança")]
        [Column("TXT_TOKEN")]
        [Required(ErrorMessage = "Informe o token de segurança docliente")]
        public string Token { get; set; }

        [Display(Name = "Data de Cadastro")]
        [Column("DAT_CADASTRO")]
        public string DataCadastro { get; set; }

        public virtual ICollection<Dispositivo> Dispositivos { get; set; }

        public Cliente()
        {
            Codigo = 0;
            Nome = "";
            Smtp = "";
            Senha = "";
            TimeZone = "";
            Token = "";

            Dispositivos = new List<Dispositivo>();
        }
    }
}