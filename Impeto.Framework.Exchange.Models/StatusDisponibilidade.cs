using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impeto.Framework.Exchange.Models
{
    [Serializable]    
    public enum StatusDisponibilidade
    {
        Desconhecido = -1,
        Livre = 0,
        Ocupado = 1,
        Erro = 2,
        EmReuniao = 3,
        EmReuniao30 = 4
    }
}
