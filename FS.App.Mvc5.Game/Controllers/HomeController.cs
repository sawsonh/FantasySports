using System.Web.Mvc;

namespace FS.App.Mvc5.Game.Controllers
{
    public class HomeController : Controller
    {
        // Landing page - #4
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