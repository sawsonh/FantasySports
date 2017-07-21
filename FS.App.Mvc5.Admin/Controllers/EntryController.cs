using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Services;
using System.Linq;
using System.Web.Mvc;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly IEntryService _entrySvc;

        public EntryController(IEntryService entrySvc)
        {
            _entrySvc = entrySvc;
        }

        // GET: Entry
        public ActionResult Index(int? id)
        {
            var model = _entrySvc.GetAll().AsQueryable().ProjectTo<EntryViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }
    }
}