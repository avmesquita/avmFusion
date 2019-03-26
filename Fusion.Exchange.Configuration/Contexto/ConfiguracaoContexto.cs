using Fusion.Exchange.Configuration.Contexto;
using Fusion.Exchange.Configuration.Models;
using Fusion.Framework.Exchange.Entity;
using Fusion.Framework.Exchange.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Fusion.Exchange.Configuration.Contexto
{
    public partial class ConfiguracaoContexto : DbContext
    {
        public ConfiguracaoContexto()
            : base("FusionExchangeEntitiesSQL")
        {
            Database.SetInitializer<ConfiguracaoContexto>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            MyGeneration(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //Database.SetInitializer(new FusionDataModelControlInitializer());
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

public class FusionDataModelControlInitializer : DropCreateDatabaseIfModelChanges<DbContext>
{
    public FusionDataModelControlInitializer()
    {
        /* CONTEXTOS RELEVANTES: */

        /* - Modelo de Dados do Projeto Sala de Reunião -> ConfiguracaoContexto */
        /* - Modelo de Dados do Controle de Usuários -> ApplicationDbContext */

        /* OPÇÔES: */

        /* 1 - PARA NÃO FAZER NADA, COMENTA TUDO */


        /* 2 - RECRIA QQ ALTERACAO */

        //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ConfiguracaoContexto>());
        //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());


        /* 3 - CRIA SOMENTE SE NAO TIVER NADA */

        // Database.SetInitializer<Context>(new CreateDatabaseIfNotExists<ConfiguracaoContexto>());
        // Database.SetInitializer<Context>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
    }

}

