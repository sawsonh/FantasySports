using AutoMapper;
using AutoMapper.QueryableExtensions;
using FS.App.Mvc5.Admin.Models;
using FS.Core.Entities;
using FS.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace FS.App.Mvc5.Admin.Controllers
{
    [Authorize]
    public class IdentityProviderController : Controller
    {
        private readonly IIdentityProviderService _idpSvc;

        public IdentityProviderController(IIdentityProviderService idpSvc)
        {
            _idpSvc = idpSvc;
        }

        // GET: IdentityProvider
        public ActionResult Index(int? id)
        {
            var model = _idpSvc.GetAll().AsQueryable().ProjectTo<IdentityProviderViewModel>().ToList();
            return id != null
                ? View("Edit", model.Where(m => m.Id == id).FirstOrDefault())
                : View(model);
        }

        // POST: IdentityProvider/{id}
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(IdentityProviderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                var idp = Mapper.Map<IdentityProvider>(model);
                _idpSvc.Update(idp);
                TempData["SuccessMessage"] = "Changes have been successfully saved";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Edit", model);
        }

        // GET: IdentityProvider/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IdentityProvider/Create
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(IdentityProviderViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Create", model);

            try
            {
                var idp = Mapper.Map<IdentityProvider>(model);
                _idpSvc.Add(idp);
                TempData["SuccessMessage"] = "New identity provider has been successfully added";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Create", model);
        }

        // POST: IdentityProvider/{id}/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(IdentityProviderViewModel model)
        {
            var idp = Mapper.Map<IdentityProvider>(model);
            _idpSvc.Remove(idp);
            TempData["SuccessMessage"] = "Identity provider has been successfully deleted";
            return RedirectToAction("Index", new { id = "" });
        }

    }
}