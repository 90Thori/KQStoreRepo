using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KQStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //   name: "Admin",
            //   url: "Admin/Admin",
            //   defaults: new { area = "Admin", controller = "Homes", action = "Index" },
            //   namespaces: new[] { "KQStore.Areas.Admin.Controllers" }
            //);

            routes.MapRoute(
           name: "Default",
           url: "{controller}/{action}/{id}",
           defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
           namespaces: new[] { "KQStore.Controllers" }
       );

            routes.MapRoute(
            name: "LoginUser",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            namespaces: new[] { "KQStore.Controllers" } 
           );

           
        }
    }
}
