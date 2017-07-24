using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Entities;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly IGameService _gameSvc;

        public GameController(IGameService gameSvc)
        {
            _gameSvc = gameSvc;
        }

        // GET: Game/{id}/Navbar
        public ActionResult Navbar(int id)
        {
            var entity = _gameSvc.GetSingle(d => d.Id == id);
            if (entity == null)
            {
                TempData["WarningMessage"] = $"Game {id} does not exist";
                return RedirectToAction("Index");
            }
            var model = Mapper.Map<GameViewModel>(entity);
            return View(model);
        }

        // GET: Game
        public ActionResult Index(int? id)
        {
            var model = _gameSvc.GetAll().AsQueryable().ProjectTo<GameViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }

        // POST: Game
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GameViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                var game = Mapper.Map<Game>(model);
                _gameSvc.Update(game);
                TempData["SuccessMessage"] = "Changes have been successfully saved";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Edit", model);
        }

        // GET: Game/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GameViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Create", model);

            try
            {
                var game = Mapper.Map<Game>(model);
                _gameSvc.Add(game);
                TempData["SuccessMessage"] = "New fantasy game has been successfully added";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Create", model);
        }

        // POST: Game/{id}/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _gameSvc.Delete(id);
            TempData["SuccessMessage"] = "Game has been successfully deleted";
            return RedirectToAction("Index", new {id = ""});
        }

        // GET: Game/{id}/Leagues
        public ActionResult Leagues(int id)
        {
            var model = _gameSvc.GetLeagues(id).AsQueryable().ProjectTo<GameLeagueViewModel>().ToList();
            return View(model);
        }

        // POST: Game/{id}/Leagues
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leagues(int id, int leagueId)
        {
            _gameSvc.RemoveLeague(id, leagueId);

            TempData["SuccessMessage"] = "League has been successfully deleted from Game";
            return RedirectToAction("Leagues");
        }

        // GET: Game/{id}/AddLeague
        public ActionResult AddLeague(int id)
        {
            var model = _gameSvc.GetLeaguesToAdd(id).AsQueryable().ProjectTo<GameLeagueViewModel>().ToList();
            return View(model);
        }

        // POST: Game/{id}/AddLeague
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLeague(int id, int leagueId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
                _gameSvc.AddLeague(id, leagueId);
                TempData["SuccessMessage"] = "League has been successfully added to game";
                return RedirectToAction("Leagues");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return RedirectToAction("Index");
        }

        // GET: Game/{id}/Periods
        public ActionResult Periods(int id, int? periodId)
        {
            var model = _gameSvc.GetPeriods(id).AsQueryable().ProjectTo<GamePeriodViewModel>().ToList();
            return periodId != null
                ? View("EditPeriod", model.Where(m => m.PeriodId == periodId).FirstOrDefault())
                : View(model);
        }

        // POST: Game/{id}/Periods
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Periods(int id, GamePeriodViewModel model)
        {
            if (DateTime.Compare(model.PickStartDateTime, model.PickEndDateTime) >= 0)
                ModelState.AddModelError("", $"Registration start date must be after its end date.");

            if (DateTime.Compare(model.ReportStartDateTime, model.ReportEndDateTime) >= 0)
                ModelState.AddModelError("", $"Play start date must be after its end date.");

            if (!ModelState.IsValid)
                return View("EditPeriod", model);

            try
            {
                var period = Mapper.Map<Period>(model);
                _gameSvc.UpdatePeriod(id, period);
                TempData["SuccessMessage"] = "Changes have been successfully saved";
                return RedirectToAction("Periods");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("EditPeriod", model);
        }

        // POST: Game/{id}/DeletePeriod
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePeriod(int id, int periodId)
        {
            try
            {
                _gameSvc.DeletePeriod(id, periodId);
                TempData["SuccessMessage"] = "Period has been successfully deleted from Game";
                return RedirectToAction("Periods");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return RedirectToAction("Periods");
        }

        // GET: Game/{id}/AddPeriod
        public ActionResult AddPeriod(int id)
        {
            return View();
        }

        // POST: Game/{id}/AddPeriod
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPeriod(int id, FormCollection collection)
        {
            var models = new List<GamePeriodViewModel>();
            try
            {
                var lastPeriodId = Convert.ToInt32(collection["periodId"]);
                for (var i = 0; i <= lastPeriodId; i++)
                {
                    var name = collection[$"Name_{i}"];
                    if (string.IsNullOrEmpty(name))
                        ModelState.AddModelError("", "Period name is required");
                    foreach (var x in new[] {"Pick", "Report"})
                    {
                        DateTime d1, d2;
                        if (DateTime.TryParse(collection[$"{x}StartDateTime_{i}"], out d1)
                            && DateTime.TryParse(collection[$"{x}EndDateTime_{i}"], out d2))
                        {
                            if (DateTime.Compare(d1, d2) >= 0)
                                ModelState.AddModelError("", $"{x} start date must be after its end date.");
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Failed to parse {x} start and/or end date.");
                        }
                    }
                    if (!ModelState.IsValid) continue;
                    int value;
                    models.Add(new GamePeriodViewModel
                    {
                        Name = name,
                        Value = int.TryParse(collection[$"Value_{i}"], out value) ? value : default(int?),
                        PickStartDateTime = Convert.ToDateTime(collection[$"PickStartDateTime_{i}"]),
                        PickEndDateTime = Convert.ToDateTime(collection[$"PickEndDateTime_{i}"]),
                        ReportStartDateTime = Convert.ToDateTime(collection[$"ReportStartDateTime_{i}"]),
                        ReportEndDateTime = Convert.ToDateTime(collection[$"ReportEndDateTime_{i}"])
                    });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            if (!ModelState.IsValid)
                return View();

            try
            {
                _gameSvc.AddPeriod(id, models.AsQueryable().ProjectTo<Period>());
                TempData["SuccessMessage"] = "Periods has been successfully added to game";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            return RedirectToAction("Periods");
        }
    }
}