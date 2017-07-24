using System.Web.Mvc;
using System.Web.Routing;

namespace FS.App.Mvc5.Admin.Areas.Nba
{
    public class NbaAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Nba";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                $"{AreaName}_DefaultActionPostfix",
                AreaName + "/{controller}/{id}/{action}",
                new {action = "Index"},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("GET")},
                new[] {$"FS.App.Mvc5.Admin.Areas.{AreaName}.Controllers"}
            );

            context.MapRoute(
                $"{AreaName}_DefaultAction",
                AreaName + "/{controller}/{id}",
                new {action = "Index", id = UrlParameter.Optional},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("GET")},
                new[] {$"FS.App.Mvc5.Admin.Areas.{AreaName}.Controllers"}
            );

            context.MapRoute(
                $"{AreaName}_HttpPostAction",
                AreaName + "/{controller}/{id}/{action}",
                new {action = "Index"},
                new {id = @"\d+", httpMethod = new HttpMethodConstraint("POST")},
                new[] {$"FS.App.Mvc5.Admin.Areas.{AreaName}.Controllers"}
            );

            context.MapRoute(
                $"{AreaName}_HttpPostActionPostfix",
                AreaName + "/{controller}/{action}",
                new {action = "Index"},
                new {httpMethod = new HttpMethodConstraint("POST")},
                new[] {$"FS.App.Mvc5.Admin.Areas.{AreaName}.Controllers"}
            );

            context.MapRoute(
                $"{AreaName}_default",
                AreaName + "/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional},
                new[] {$"FS.App.Mvc5.Admin.Areas.{AreaName}.Controllers"}
            );
        }
    }
}