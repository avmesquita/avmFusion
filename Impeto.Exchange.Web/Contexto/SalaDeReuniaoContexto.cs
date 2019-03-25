using Impeto.Exchange.Web.Contexto;
using Impeto.Framework.Exchange.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Impeto.Exchange.Web.Contexto
{
    public partial class SalaDeReuniaoContexto : DbContext
    {
        public SalaDeReuniaoContexto()
            : base("ImpetoExchangeEntitiesSQL") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Caso queira escrever regra para criação de tabelas e dados
            MyGeneration(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Inicializador Padrão
            //Database.SetInitializer(new MyInitializer());
        }

        public int SaveChanges(bool refreshOnConcurrencyException, RefreshMode refreshMode = RefreshMode.ClientWins)
        {
            try
            {
                return SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (DbEntityEntry entry in ex.Entries)
                {
                    if (refreshMode == RefreshMode.ClientWins)
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    else
                        entry.Reload();
                }
                return SaveChanges();
            }
        }

        private void MyGeneration(DbModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<Dispositivo>()
                        .HasRequired<Cliente>(s => s.Dispositivo)
                        .WithMany(s => s.Dispositivos);
            */
        }

        public System.Data.Entity.DbSet<Cliente> ClienteModels { get; set; }

        public System.Data.Entity.DbSet<Dispositivo> DispositivoModels { get; set; }

        public System.Data.Entity.DbSet<ExchangeAvaiability> DisponibilidadeModels { get; set; }

        public System.Data.Entity.DbSet<ExchangeAvaiabilityStatus> DisponibilidadeStatusModels { get; set; }

        public System.Data.Entity.DbSet<ExchangeAvaiabilityStatusDetail> DisponibilidadeStatusDetalheModels { get; set; }

        public System.Data.Entity.DbSet<ExchangeAvaiabilityEmail> DisponibilidadeEmailModels { get; set; }

        public System.Data.Entity.DbSet<LogEvento> LogEventoModels { get; set; }
    }
}

public class MyInitializer : DropCreateDatabaseIfModelChanges<SalaDeReuniaoContexto>
{
    public MyInitializer()
    {
        // SE HOUVER MUDANÇAS, NÃO FAZER NADA E CONTINUAR
    }

}
