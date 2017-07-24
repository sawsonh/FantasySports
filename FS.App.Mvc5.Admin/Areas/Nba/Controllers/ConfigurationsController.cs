using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Entities;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Areas.Nba.Controllers
{
    [Authorize]
    public class ConfigurationsController : Controller
    {
        private readonly IConfigService _cfgSvc;

        public ConfigurationsController(IConfigService cfgSvc)
        {
            _cfgSvc = cfgSvc;
        }

        // GET: Nba/Configurations
        public ActionResult Index(int? id)
        {
            var model = _cfgSvc.GetList(appSetting => appSetting.KeyName.ToLower().StartsWith("nba_")).AsQueryable()
                .ProjectTo<ConfigurationViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }

        // GET: POST/Configurations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfigurationViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);
            try
            {
                _cfgSvc.Update(Mapper.Map<AppSetting>(model));
                TempData["SuccessMessage"] = "Changes have been successfully saved";
                return RedirectToAction("Index", new {id = ""});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Edit", model);
        }
    }
}