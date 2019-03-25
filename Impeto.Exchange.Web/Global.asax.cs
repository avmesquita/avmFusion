using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace Impeto.Exchange.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            this.EndRequest += MvcApplication_EndRequest;
            base.Init();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        void MvcApplication_EndRequest(object sender, EventArgs e)
        {
            var request = this.Request;
            var response = this.Response;

            if (request.RawUrl.Contains("Content/"))
            {
                response.Cache.SetExpires(DateTime.Now.AddMinutes(7200));
                response.Cache.SetLastModified(DateTime.Now);
                response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                //request.Headers.Add("Content-Type", "application/x-gzip");
                //request.Headers.Add("Content-Encoding", "gzip");
            }

            if (request.RawUrl.Contains("bundles/"))
            {
                response.Cache.SetExpires(DateTime.Now.AddMinutes(7200));
                response.Cache.SetLastModified(DateTime.Now);
                response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
                //request.Headers.Add("Content-Type", "application/x-gzip");
                //request.Headers.Add("Content-Encoding", "gzip");
            }

        }

    }
}
