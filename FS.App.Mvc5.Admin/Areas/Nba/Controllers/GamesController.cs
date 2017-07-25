using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Areas.Nba.Models;
using FS.Core.Entities;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Areas.Nba.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly IDataService<vNbaGame> _nbaGameSvc;
        private readonly INbaService _nbaSvc;

        public GamesController(INbaService nbaSvc, IDataService<vNbaGame> nbaGameSvc)
        {
            _nbaSvc = nbaSvc;
            _nbaGameSvc = nbaGameSvc;
        }

        // GET: Nba/Games
        public ActionResult Index()
        {
            var model = _nbaGameSvc.GetAll().OrderByDescending(nba => nba.DateTime).AsQueryable()
                .ProjectTo<NbaGameViewModel>().ToList();
            return View(model);
        }

        // GET: POST/Games/Resync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resync(DateTime dateTime)
        {
            _nbaSvc.Run(dateTime);
            TempData["SuccessMessage"] = "Resync job has been submitted";
            return RedirectToAction("Index");
        }
    }
}