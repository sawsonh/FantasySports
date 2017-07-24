using System.Web.Mvc;
using System.Web.Routing;

namespace FS.App.Mvc5.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DefaultActionPostfix",
                "{controller}/{id}/{action}",
                new {action = "Index"},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("GET")},
                new[] {"FS.App.Mvc5.Admin.Controllers"}
            );

            routes.MapRoute(
                "DefaultAction",
                "{controller}/{id}",
                new {action = "Index", id = UrlParameter.Optional},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("GET")},
                new[] {"FS.App.Mvc5.Admin.Controllers"}
            );

            routes.MapRoute(
                "HttpPostAction",
                "{controller}/{id}/{action}",
                new {action = "Index"},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("POST")},
                new[] {"FS.App.Mvc5.Admin.Controllers"}
            );

            routes.MapRoute(
                "HttpPostActionPostfix",
                "{controller}/{action}",
                new {action = "Index"},
                new {httpMethod = new HttpMethodConstraint("POST")},
                new[] {"FS.App.Mvc5.Admin.Controllers"}
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Account", action = "Index", id = UrlParameter.Optional},
                new[] {"FS.App.Mvc5.Admin.Controllers"}
            );
        }
    }
}