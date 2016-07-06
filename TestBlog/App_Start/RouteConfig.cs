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

            routes.MapRoute(
                 "Manage Permission",
                 "{controller}/{action}/role/{roleid}",
                 new { controller = "Manage", action = "Permissions", roleid = UrlParameter.Optional }
             );

            routes.MapRoute(
                "Assign Role",
                "{controller}/{action}/id/{id}/role/{roleid}",
                new { controller = "Manage", action = "AssignRole", id = UrlParameter.Optional, roleid = UrlParameter.Optional }
            );

            routes.MapRoute(
               "Remove Role",
               "{controller}/{action}/uid/{uid}/role/{roleid}",
               new { controller = "Manage", action = "RemoveFromRole", uid = UrlParameter.Optional, roleid = UrlParameter.Optional }
           );
        }
    }
}
