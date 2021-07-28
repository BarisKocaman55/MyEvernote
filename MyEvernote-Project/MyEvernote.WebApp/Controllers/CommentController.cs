using MyEvernote.BusinessLayer.Concretes;
using MyEvernote.Entities;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class CommentController : Controller
    {
        private NotesManager noteManager = new NotesManager();
        private CommentManager commentManager = new CommentManager();
        public ActionResult ShowNoteComments(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Note note = noteManager.Find(x => x.Id == id.Value);
            Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id.Value);

            if(note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments", note.Comments);
        }

        [Auth]
        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = commentManager.Find(x => x.Id == id.Value);

            if(comment == null)
            {
                return HttpNotFound();
            }

            comment.Text = text;
            if(commentManager.Update(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        public ActionResult Delete(int? id) 
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = commentManager.Find(x => x.Id == id.Value);

            if(comment == null)
            {
                return HttpNotFound();
            }

            if(commentManager.Delete(comment) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Auth]
        [HttpPost]
        public ActionResult Create(Comment comment, int? noteid)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (noteid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Note note = noteManager.Find(x => x.Id == noteid.Value);

                if (note == null)
                {
                    return HttpNotFound();
                }

                comment.Note = note;
                comment.Owner = CurrentSession.user;

                if(commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}