using System.Web.Mvc;

namespace FS.App.Mvc5.Game.Controllers
{
    public class HomeController : Controller
    {
        // Landing page - #6
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
