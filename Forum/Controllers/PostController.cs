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
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
           // return View("ListQuestions");
            return RedirectToAction("ListQuestions");
        }


        public ActionResult ListQuestions()
        {
            using (var database = new ForumDbContext())
            {
                var questions = database.Posts.Where(x => x.ParentPostId == null).Include(a => a.Author).ToList();
                return View(questions);
            }

        }

        
        public ActionResult ListAnswers(int id,int? reply)
        {
            using (var database = new ForumDbContext())
            {
                var answers = database.Posts.Where(x => x.ParentPostId == id).Include(a => a.Author).ToList();

                ViewBag.question = database.Posts.Where(x => x.PostId == id).Include(a => a.Author).First();

                
                if (reply == 1)
                    
                    ViewBag.reply = 1;
                else
                    ViewBag.reply = 0;

                List<Object> model = new List<object>();

                                                       

                model.Add(answers);

                Post post = new Post();
                model.Add(post);

                return View(model);
            }
        }



        [Authorize]
        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreatePost(Post post,int? id)
        {

            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {
                    var authorId = database.Users.Where(x => x.UserName == this.User.Identity.Name).First().Id;
                    post.AuthorId = authorId;
                    post.ParentPostId = id;
                    database.Posts.Add(post);
                    database.SaveChanges();

                    if(id==null)
                    return RedirectToAction("ListQuestions");
                    else
                    return RedirectToAction("ListAnswers","Post", new { id = id });
                }


            }

            return View(post);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ForumDbContext())
            {
                var post = database.Posts.Where(p=>p.PostId == id).Include(p => p.Author).First();
                      

                if (post == null)
                {
                    return HttpNotFound();
                }
                 
                return View(post);

            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            using (var database = new ForumDbContext())
            {
                var post = database.Posts.Where(x => x.PostId == id).Include(a => a.Author).First();
                if (post == null)
                {
                    return HttpNotFound();
                }

                foreach (var item in database.Posts.Where(x=>x.ParentPostId==post.PostId))
                {
                   database.Posts.Remove(item);
                }

                database.Posts.Remove(post);
                database.SaveChanges();
            }

            return RedirectToAction("ListQuestions","Post");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new ForumDbContext())
            {
                var post = database.Posts.Where(x => x.PostId == id).First();
                if (post == null)
                    return HttpNotFound();

                if (!isUserAuthorizedToEdit(post))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                

                return View(post);

            }

        }

        [HttpPost]
        public ActionResult Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {

                    var postElem = database.Posts.FirstOrDefault(p=>p.PostId == post.PostId);

                    postElem.Title = post.Title;
                    postElem.Content = post.Content;

                    database.Entry(postElem).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");

                }
            }

            return View(post);

        }

        private bool isUserAuthorizedToEdit(Post post)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = post.isAuthor(this.User.Identity.Name);
            
            return isAdmin || isAuthor;

        }



    }
}