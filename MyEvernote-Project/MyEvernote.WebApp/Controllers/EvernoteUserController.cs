using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer.Concretes;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;

namespace MyEvernote.WebApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class EvernoteUserController : Controller
    {
        private EverNoteUserManager everNoteUserManager = new EverNoteUserManager();
        public ActionResult Index()
        {
            return View(everNoteUserManager.List());
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id.Value);

            if(evernoteUser == null)
            {
                return HttpNotFound();
            }

            return View(evernoteUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> result = everNoteUserManager.Insert(evernoteUser);

                if (result.Errors.Count > 0)
                {
                    result.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(evernoteUser);
                }

                return RedirectToAction("Index");
            }

            return View(evernoteUser);
        }

        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id.Value);

            if(evernoteUser == null)
            {
                return HttpNotFound();
            }

            return View(evernoteUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvernoteUser evernoteUser)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> result = everNoteUserManager.Update(evernoteUser);

                if(result.Errors.Count > 0)
                {
                    result.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(evernoteUser);
                }

                return RedirectToAction("Index");
            }
            return View(evernoteUser);
        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id.Value);

            if(evernoteUser == null)
            {
                return HttpNotFound();
            }

            return View(evernoteUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvernoteUser evernoteUser = everNoteUserManager.Find(x => x.Id == id);
            everNoteUserManager.Delete(evernoteUser);

            return RedirectToAction("Index");
        }
    }
}