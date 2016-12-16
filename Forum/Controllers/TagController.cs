using Forum.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public ActionResult List(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            }

            using (var database = new ForumDbContext())
            {
                var questions = database.Tags
                    .Include(t => t.Posts.Select(a => a.Tags))
                    .Include(t => t.Posts.Select(a => a.Author))
                    .FirstOrDefault(x => x.Id == id)
                    .Posts
                    .ToList();


                return View(questions);
            }

        }


    }
}