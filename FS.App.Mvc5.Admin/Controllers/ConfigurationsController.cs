using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Services;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class ConfigurationsController : Controller
    {
        private readonly IConfigService _cfgSvc;

        public ConfigurationsController(IConfigService cfgSvc)
        {
            _cfgSvc = cfgSvc;
        }

        // GET: Configurations
        public ActionResult Index(int? id)
        {
            var model = _cfgSvc.GetAll().AsQueryable().ProjectTo<ConfigurationViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }

        // POST: Configurations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfigurationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appSetting = _cfgSvc.GetSingle(d => d.Id == model.Id);
                if (appSetting == null)
                {
                    ModelState.AddModelError("", $"Cannot find app settings for Id {model.Id}");
                }
                else
                {
                    var hasChanged = !appSetting.Value.Equals(model.Value, StringComparison.OrdinalIgnoreCase);
                    if (hasChanged)
                    {
                        appSetting.Value = model.Value;
                        _cfgSvc.Update(appSetting);
                        TempData["SuccessMessage"] = "Changes have been successfully saved";
                        return RedirectToAction("Index", new {id = ""});
                    }
                    TempData["WarningMessage"] = "There were no changes saved";
                    return RedirectToAction("Index", new {id = ""});
                }
            }
            return View("Edit", model);
        }
    }
}