using System;
using System.Collections.Generic;
using MyEvernote.Common;
using MyEvernote.Entities;
using System.Linq;
using System.Web;
using MyEvernote.WebApp.Models;

namespace MyEvernote.WebApp.Init
{
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            //if(HttpContext.Current.Session["login"] != null)
            //{
            //    EvernoteUser user = HttpContext.Current.Session["login"] as EvernoteUser;
            //    return user.Username;
            //}

            EvernoteUser user = CurrentSession.user;

            if(user != null)
            {
                return user.Username;
            }

            return "system";
        }
    }
}