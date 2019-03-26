using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using System.Security.Principal;

namespace Fusion.Exchange.Configuration.Extensions
{
    public static class IdentityExtensions
    {

        public static string GetNome(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Nome");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetCodigoCliente(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CodigoCliente");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetFirstDevice(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstDevice");
            return (claim != null) ? claim.Value : string.Empty;
        }


    }
}