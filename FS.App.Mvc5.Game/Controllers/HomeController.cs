using System.Web.Mvc;

namespace FS.App.Mvc5.Game.Controllers
{
    public class HomeController : Controller
    {
        // Landing page
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