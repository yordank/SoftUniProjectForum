using Forum.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Forum.Controllers.Admin
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            using (var database = new ForumDbContext())
            {
                var categories = database.Categories.ToList();

                return View(categories);
            }
                
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {
                    database.Categories.Add(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }


            }

            return View(category);

        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ForumDbContext())
            {
                var category = database.Categories.FirstOrDefault(x => x.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {
                    database.Entry(category).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(category);

        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ForumDbContext())
            {
                var category = database.Categories.FirstOrDefault(x => x.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }


        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteProcess(int? id)
        {


            using (var database = new ForumDbContext())
            {
                var category = database.Categories.FirstOrDefault(x => x.Id == id);
                var categoryPosts = category.Posts.ToList();

                foreach (var post in categoryPosts)
                {
                    foreach (var item in database.Posts.Where(x=>x.ParentPostId==post.PostId))
                    {
                        database.Posts.Remove(item);
                    }
                    database.Posts.Remove(post);

                }
                database.Categories.Remove(category);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
        }


    }
}