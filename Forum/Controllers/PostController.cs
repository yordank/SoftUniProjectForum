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
            return View("ListQuestions");
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


         


    }
}