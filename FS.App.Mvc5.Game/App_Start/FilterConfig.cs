using System.Web;
using System.Web.Mvc;

namespace FS.App.Mvc5.Game
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
