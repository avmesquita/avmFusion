using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Database.SQLite
{
    public class Plano
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public int QtdeDispositivos { get; set; }
        public DateTime InicioVigencia { get; set; }
        public DateTime FimVigencia { get; set; }
        public Decimal Valor { get; set; }

        public Plano()
        {
            Codigo = 0;
            Titulo = "";
            QtdeDispositivos = 0;
            InicioVigencia = DateTime.MinValue;
            FimVigencia = DateTime.MaxValue;
            Valor = 0;
        }
    }
}
