using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Impeto.Framework.Database.SQLite
{
    public class Dispositivo
    {
        public int Codigo { get; set; }
        public Cliente Cliente { get; set; }
        public string Nome { get; set; }
        public string SSID_ID { get; set; }
        public string SSID_PASS { get; set; }
        public string MAC { get; set; }
        public string Token { get; set; }
        public int TempoFixo { get; set; }
        public int TempoEvento { get; set; }
        public DateTime DataAtivacao { get; set; }
        public bool Ativo { get; set; }

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
            DataAtivacao = DateTime.MinValue;
            Ativo = true;

            Cliente = new Cliente();
        }
    }
}