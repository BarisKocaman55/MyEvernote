using System;
using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Concretes;
using MyEvernote.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.Filters;

namespace MyEvernote.WebApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class CategoryController : Controller
    {
        private CategoryManager categoryManager = new CategoryManager();
        //public ActionResult Select(int? id)
        //{
        //    if(id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    CategoryManager categoryManager = new CategoryManager();
        //    //Category category = categoryManager.GetCategoryById(id.Value);
        //    Category category = categoryManager.Find(x => x.Id == id.Value);

        //    if(category == null)
        //    {
        //        return HttpNotFound();
                
        //    }

        //    TempData["model"] = category.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList();
        //    return RedirectToAction("Index", "Home");
        //}

        public ActionResult Index()
        {
            //return View(CacheHelper.GetCategoriesFromCache());
            return View(categoryManager.List());
        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryManager.Find(x => x.Id == id.Value);
            if(category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                categoryManager.Insert(category);
                CacheHelper.ClearCache("category-cache");
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryManager.Find(x => x.Id == id.Value);

            if(category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            ModelState.Remove("CreateOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");
            if (ModelState.IsValid)
            {
                Category cat = categoryManager.Find(x => x.Id == category.Id);
                cat.Title = category.Title;
                cat.Description = category.Description;

                categoryManager.Update(cat);
                CacheHelper.ClearCategoryCache();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = categoryManager.Find(x => x.Id == id.Value);
            if(category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = categoryManager.Find(x => x.Id == id);
            categoryManager.Delete(category);
            CacheHelper.ClearCache("category-cache");

            return RedirectToAction("Index");
        }
    }
}