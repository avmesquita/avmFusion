using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]
    public class Conflito
    {        
        public TipoConflito TipoConflito { get; set; }

        public StatusLegadoLivreOcupado StatusLegadoLivreOcupado { get; set; }

        public int NumberOfMembers { get; set; }

        public int NumberOfMembersAvailable { get; set; }

        public int NumberOfMembersWithConflict { get; set; }

        public int NumberOfMembersWithNoData { get; set; }
    }
}
