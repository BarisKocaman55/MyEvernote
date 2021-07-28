using MyEvernote.BusinessLayer.Concretes;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyEvernote.WebApp.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("category-cache") as List<Category>;
            
            if(result == null)
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();
                WebCache.Set("category-cache", categoryManager.List(), 20, true);
            }

            return result;
        }

        public static void ClearCache(string key)
        {
            WebCache.Remove(key);
        }

        public static void ClearCategoryCache()
        {
            WebCache.Remove("category-cache");
        }
    }
}