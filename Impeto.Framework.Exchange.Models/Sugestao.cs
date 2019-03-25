using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]
    public class Sugestao
    {
        public DateTime Date { get; set; }

        public SugestaoQualidade Quality { get; set; }

        public DateTime DataHoraDisponivel { get; set; }
    }
}
