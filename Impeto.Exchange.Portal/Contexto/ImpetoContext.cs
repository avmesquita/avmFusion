using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Impeto.Exchange.Portal.Contexto
{
    public class ImpetoContext : DbContext
    {
        public ImpetoContext()
            : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public class MyInitializer : DropCreateDatabaseIfModelChanges<ImpetoContext>
        {
            public MyInitializer()
            {
                // PARA NÃO FAZER NADA, COMENTA TUDO


                // RECRIA QQ ALTERACAO
                //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AgendamentoContexto>());                

                // CRIA SOMENTE SE NAO TIVER NADA
                //Database.SetInitializer<Context>(new CreateDatabaseIfNotExists<Context>());
            }

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //MyGeneration(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            Database.SetInitializer(new MyInitializer());
        }

        private void MyGeneration(DbModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<Dispositivo>()
                        .HasRequired<Cliente>(s => s.Dispositivo)
                        .WithMany(s => s.Dispositivos);
            */
        }
    }
}