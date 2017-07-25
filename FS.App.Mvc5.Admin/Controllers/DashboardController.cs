using System.Linq;
using System.Web.Mvc;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDependencyResolver _resolver;

        public DashboardController()
        {
            _resolver = DependencyResolver.Current;
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult UserCount()
        {
            var svc = _resolver.GetService(typeof(IUserService)) as IUserService;
            return Json(svc.GetAll().Count(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EntryCount()
        {
            var svc = _resolver.GetService(typeof(IEntryService)) as IEntryService;
            return Json(svc.GetAll().Count(), JsonRequestBehavior.AllowGet);
        }
    }
}