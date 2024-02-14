using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebPKthucung
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetHuyenList",
                url: "Users/GetHuyenList",
                defaults: new { controller = "Users", action = "GetHuyenList" }
            );

            routes.MapRoute(
                name: "GetXaList",
                url: "Users/GetXaList",
                defaults: new { controller = "Users", action = "GetXaList" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "Index", id = UrlParameter.Optional }
            );


        }

    }
}
