using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _usrSvc;

        public UserController(IUserService usrSvc)
        {
            _usrSvc = usrSvc;
        }

        // GET: User
        public ActionResult Index(int? id)
        {
            var model = _usrSvc.GetAll().AsQueryable().ProjectTo<UserViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }
    }
}