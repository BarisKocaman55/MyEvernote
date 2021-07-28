using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Concretes;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ValueObjects;
using MyEvernote.WebApp.Filters;
using MyEvernote.WebApp.Models;
using MyEvernote.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NotesManager noteManager = new NotesManager();
        private EverNoteUserManager manager = new EverNoteUserManager();
        private CategoryManager categoryManager = new CategoryManager();

        //public ActionResult Index()
        //{
        //    //BusinessLayer.Test test = new BusinessLayer.Test();
        //    //test.InserTest();
        //    //test.UpdateTest();
        //    //test.InsertNote();
        //    //test.CommnetTest();

        //    if(TempData["model"] != null)
        //    {
        //        return View(TempData["model"] as List<Note>);
        //    }


        //    return View(noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
        //}

        public ActionResult Index()
        {
            return View(noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
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

            List<Note> notes = category.Notes.Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList();

            return View("Index", notes);
        }

        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> result = manager.LoginUser(model);

                if (result.Errors.Count > 0)
                {
                    result.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    if(result.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActivate) != null)
                    {
                        ViewBag.SetLink = "http://Home/Activate/1234-4567-7890";
                    }

                    return View(model);
                }

                //Session["login"] = result.Result; // Session ile bilgiler html sayfasında kullanılabilir.
                CurrentSession.Set<EvernoteUser>("login", result.Result);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = manager.RegisterUser(model);

                if(res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                //EvernoteUser user = null;

                //try
                //{
                //    manager.RegisterUser(model);
                //} catch(Exception ex)
                //{
                //    ModelState.AddModelError("", ex.Message);
                //}

                //if(user == null)
                //{
                //    return View(model);
                //}

                OkViewModel notifyModel = new OkViewModel()
                {
                    Title = "Successfully Registered...",
                    RedirectingUrl = "/Home/Login",
                    RedirectingTimeout = 5000,
                };

                notifyModel.Items.Add("Please activate your account by clicking the link we sent you just now!!!");

                return View("Ok", notifyModel);
                //if(model.Username == "aaa")
                //{
                //    ModelState.AddModelError("", "Username already exists !!!");

                //}

                //if(model.Email == "aaa@aa.com")
                //{
                //    ModelState.AddModelError("", "Email alredy exists !!!");

                //}

                //foreach(var item in ModelState)
                //{
                //    if(item.Value.Errors.Count > 0)
                //    {
                //        return View(model);
                //    }
                //}
            }
            return View(model);
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            //EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> result = manager.GetUserById(CurrentSession.user.Id);

            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyModel = new ErrorViewModel()
                {
                    Title = "Error Occured...",
                    Items = result.Errors
                };

                return View("error", errorNotifyModel);
            }

            return View(result.Result);
        }

        [Auth]
        public ActionResult EditProfile()
        {
            //EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> result = manager.GetUserById(CurrentSession.user.Id);

            if (result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyModel = new ErrorViewModel()
                {
                    Title = "Error Occured...",
                    Items = result.Errors
                };

                return View("error", errorNotifyModel);
            }

            return View(result.Result);
        }

        [HttpPost]
        public ActionResult EditProfile(EvernoteUser model, HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" || ProfileImage.ContentType == "image/jpg" || ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";

                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFilename = filename;
                }

                BusinessLayerResult<EvernoteUser> result = manager.UpdateProfile(model);

                if (result.Errors.Count > 0)
                {
                    ErrorViewModel errorModel = new ErrorViewModel()
                    {
                        Items = result.Errors,
                        Title = "Error occured while updating the profile...",
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    return View("Error", errorModel);
                }

                //Session["login"] = result.Result; // Updating Session
                CurrentSession.Set<EvernoteUser>("login", result.Result);
                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        [Auth]
        public ActionResult DeleteProfile()
        {
            EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> result = manager.RemoveUserById(currentUser.Id);

            if(result.Errors.Count > 0)
            {
                ErrorViewModel errorModel = new ErrorViewModel()
                {
                    Items = result.Errors,
                    Title = "Error while deleting the profile !!!",
                    RedirectingUrl = "/Home/ShowProfile"
                };

                return View("Error", errorModel);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> result = manager.ActivateUser(id);

            if(result.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyModel = new ErrorViewModel()
                {
                    Title = "Invalid Process...",
                    Items = result.Errors
                };

                TempData["errors"] = result.Errors;
                return View("Error", errorNotifyModel);
            }

            OkViewModel okNotifyModel = new OkViewModel()
            {
                Title = "Account Activated...",
                RedirectingUrl = "/Home/Login",
            };

            okNotifyModel.Items.Add("Your Account has been activated. You can share notes and like others now...");

            return View("Ok", okNotifyModel);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}