using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TestBlog
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "",
                new { controller = "Blog", action = "Index" }
            );

            routes.MapRoute(
                "Blog Details",
                "{controller}/{action}/{id}/{title}",
                new { controller = "Blog", action = "Details", id = UrlParameter.Optional, title = UrlParameter.Optional }
            );
        }
    }
}
