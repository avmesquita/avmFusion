using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Fusion.Exchange.Arduino
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ArduinoPost"
                , routeTemplate: "Fusion.Exchange.Arduino/api/Arduino/Post/{smtp}/{haspeople}" // /{idCliente}"
																							   //, defaults: new { idCliente = RouteParameter.Optional }
			);
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "Fusion.Exchange.Arduino/api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
