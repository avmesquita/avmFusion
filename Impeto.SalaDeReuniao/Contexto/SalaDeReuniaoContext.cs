using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Impeto.SalaDeReuniao.Entity;

namespace Impeto.SalaDeReuniao.Contexto
{
    public class SalaDeReuniaoContext : ImpetoContext
    {
        public DbSet<Cliente> ClienteModels { get; set; }
        public DbSet<Dispositivo> DispositivoModels { get; set; }
    }
}