using Impeto.Exchange.Portal.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Impeto.Exchange.Portal.Contexto
{
    public class SalaDeReuniaoContexto : ImpetoContext
    {
        public SalaDeReuniaoContexto()
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Cliente> ClienteModels { get; set; }
        public DbSet<Dispositivo> DispositivoModels { get; set; }

    }
}