using Forum.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var database = new ForumDbContext())
            {
                var categories = database.Categories.Include(c=>c.Posts).ToList();

                return View(categories);
            }
           
        }
       

    }
}