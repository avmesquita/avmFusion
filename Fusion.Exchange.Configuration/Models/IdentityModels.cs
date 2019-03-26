using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using Fusion.Framework.Exchange.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Fusion.Exchange.Configuration.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //[Table("TB_USUARIO")]
    public class ApplicationUser : IdentityUser
    {
        //Extended Properties
        [Column("TXT_NOME")]
        public string Nome { get; set; }

        [Column("TXT_FIRST_DEVICE")]
        public string FirstDevice { get; set; }

        [Column("COD_CLIENTE")]
        public int CodigoCliente { get; set; }

        //[ForeignKey("CodigoCliente")]
        //public virtual Cliente Cliente { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Nome", this.Nome));
            userIdentity.AddClaim(new Claim("FirstDevice", this.FirstDevice));
            userIdentity.AddClaim(new Claim("CodigoCliente", this.CodigoCliente.ToString()));

            return userIdentity;
        }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("FusionExchangeEntitiesSQL", throwIfV1Schema: false)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Database.SetInitializer(new FusionDataModelControlInitializer());
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}