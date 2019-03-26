using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Impeto.Framework.Database.SQLite
{
    public class Cliente
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Smtp { get; set;  }
        public string Senha { get; set;  }
        public string TimeZone { get; set; }
        public string Token { get; set; }
        public Plano Plano { get; set; }

        public Cliente()
        {
            Codigo = 0;
            Nome = "";
            Smtp = "";
            Senha = "";
            TimeZone = "";
            Token = "";
            Plano = new Plano();
        }

    }
}