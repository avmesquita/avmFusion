using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]
    public class SugestaoDataDisponivel
    {        
        public List<Conflito> Conflitos { get; set; }
        public bool IsHorarioComercial { get; set;  }
        public DateTime DataEvento { get; set;  }
        public SugestaoQualidade Qualidade { get; set;  }
    }
}
