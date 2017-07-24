using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Areas.Nba.Models;
using FS.Core.Entities;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Areas.Nba.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly IDataService<vNbaTeamStat> _nbaTeamStatSvc;
        private readonly IDataService<NbaTeam> _nbaTeamSvc;

        public TeamsController(IDataService<NbaTeam> nbaTeamSvc, IDataService<vNbaTeamStat> nbaTeamStatSvc)
        {
            _nbaTeamSvc = nbaTeamSvc;
            _nbaTeamStatSvc = nbaTeamStatSvc;
        }

        // GET: Nba/Teams
        public ActionResult Index(int? id)
        {
            var model = _nbaTeamSvc.GetAll().AsQueryable().ProjectTo<NbaTeamViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.TeamId == id).FirstOrDefault())
                : View(model);
        }

        // GET: POST/Teams
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NbaTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _nbaTeamSvc.GetSingle(d => d.TeamId == model.TeamId);
                if (entity == null)
                {
                    ModelState.AddModelError("", $"Cannot find team for Id {model.TeamId}");
                }
                else
                {
                    var newEntity = Mapper.Map<NbaTeam>(model);
                    var hasChanged = !newEntity.CompareTo(entity, "Id");
                    if (hasChanged)
                    {
                        newEntity.CopyTo(entity, "Id");
                        _nbaTeamSvc.Update(entity);
                        TempData["SuccessMessage"] = "Changes have been successfully saved";
                        return RedirectToAction("Index", new {id = ""});
                    }
                    TempData["WarningMessage"] = "There were no changes saved";
                    return RedirectToAction("Index", new {id = ""});
                }
            }
            return View("Edit", model);
        }

        // GET: Nba/Teams/Stats
        public ActionResult Stats(int? id)
        {
            var model = _nbaTeamStatSvc.GetList(stat => stat.TeamId == id || id == null).AsQueryable()
                .ProjectTo<NbaTeamStatViewModel>().ToList();
            return View(model);
        }
    }
}