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
         static int postsPerPage = 10;
         static int pagingCoeff = 2;
        // GET: Post
        public ActionResult Index(int categoryId)
        {
           // return View("ListQuestions");
            return RedirectToAction("ListQuestions","Post", new { categoryId = categoryId });
        }


        public ActionResult ListQuestions(int categoryId,int page)
        {
            using (var database = new ForumDbContext())
            {
                var questions = database.Posts.Where(x => x.ParentPostId == null && x.CategoryId==categoryId).Include(a => a.Author).OrderBy(x => -x.PostId).ToList();

                int pageCount = questions.Count() / postsPerPage + 1;
                ViewBag.pages = pageCount;

                ViewBag.pagingElems = this.pagingNumbers(pageCount, page);

                ViewBag.categoryId = categoryId;
                
               

                int count =Math.Min( questions.Count(),page*postsPerPage);

                var questionsPerPage = questions.Skip((page - 1) * postsPerPage).Take(count).ToList();

                ViewBag.answersPerTopic = answersPerTopic(questionsPerPage, database);

                return View(questionsPerPage);
            }

        }

        private List<int> answersPerTopic(List<Post> questions,ForumDbContext database)
        {
            List<int> answersPerTopic = new List<int>();

            foreach (var item in questions)
            {
                var count = database.Posts.Where(x => x.ParentPostId == item.PostId).Count();
                answersPerTopic.Add(count);
            }

            return answersPerTopic;
        }

        public ActionResult ListAnswers(int id,int? reply,int page)
        {
            using (var database = new ForumDbContext())
            {
                var answers = database.Posts.Where(x => x.ParentPostId == id).Include(a => a.Author).ToList();

                Post question = database.Posts.Where(x => x.PostId == id).Include(p => p.Author).Include(p=>p.Tags).First();

                ////////////////
                if (reply == null)
                {
                    question.Views++;
                    database.Entry(question).State = EntityState.Modified;
                    database.SaveChanges();
                }
               ///////////////

                ViewBag.question = question;

                
                if (reply == 1)
                    ViewBag.reply = 1;
                else
                    ViewBag.reply = 0;

                ViewBag.categoryId = question.CategoryId;

                ViewBag.page = page;

                int pageCount = answers.Count() / postsPerPage + 1;
                ViewBag.pages = pageCount;

                ViewBag.pagingElems = this.pagingNumbers(pageCount, page);

                List<Object> model = new List<object>();

               
                int count =Math.Min( answers.Count(),page*postsPerPage);

                var answersPerPage = answers.Skip((page-1)*postsPerPage).Take(count).ToList();

                model.Add(answersPerPage);

                PostViewModel post = new PostViewModel();

                model.Add(post);

                return View(model);
            }
        }

        private List<Post>getPagePost(List<Post>list)
        {
            return list;

        }


        [Authorize]
        public ActionResult CreatePost(int? categoryId)
        {
            if (categoryId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.categoryId = categoryId;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreatePost(PostViewModel post,int? id,int? page)
        {
           

            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {
                    var authorId = database.Users.Where(x => x.UserName == this.User.Identity.Name).First().Id;
                    post.AuthorId = authorId;
                    post.ParentPostId = id;

                    var newPost = new Post(post.AuthorId, post.Title, post.Content, post.CategoryId,post.ParentPostId);

                    this.SetArticleTags(newPost, post.Tags, database);

                    if (id == null)
                    {
                        
 
                        database.Posts.Add(newPost);
                        database.SaveChanges();

                        return RedirectToAction("ListQuestions", "Post", new { categoryId = post.CategoryId,page=1 });
                    }

                    else
                    {
                        database.Posts.Add(newPost);
                        database.SaveChanges();
                      
                        return RedirectToAction("ListAnswers", "Post", new { id = id, categoryId = post.CategoryId, page = page });
                    }
                    
                  

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


                if (!isUserAuthorizedToEdit(post))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

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

            int categoryId;

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

                categoryId = post.CategoryId;

                database.Posts.Remove(post);
                database.SaveChanges();
            }

            return RedirectToAction("ListQuestions","Post", new { categoryId = categoryId,page=1 });
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


                var model = new PostViewModel();
                model.PostId = post.PostId;
                model.Title = post.Title;
                model.AuthorId = post.AuthorId;
                model.Content =  post.Content;
                model.CategoryId = post.CategoryId;
                model.ParentPostId = post.ParentPostId;

                model.Tags = string.Join(", ", post.Tags.Select(t => t.Name));


                return View(model);

            }

        }

        [HttpPost]
        public ActionResult Edit(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                using (var database = new ForumDbContext())
                {

                    var postElem = database.Posts.FirstOrDefault(p=>p.PostId == post.PostId);

                    postElem.Title = post.Title;
                    postElem.Content = post.Content;

                    this.SetArticleTags(postElem, post.Tags, database);

                    database.Entry(postElem).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("ListQuestions","Post", new { categoryId = postElem.CategoryId,page=1 });

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

        private List<int>pagingNumbers(int count,int current)
        {
            List<int> pagesDisplayed = new List<int>();

            pagesDisplayed.Add(1);

            for (int i = 2; i <= count-1; i++)
            {
                if (Math.Abs(current - i) <= pagingCoeff)
                    pagesDisplayed.Add(i);
            }

            pagesDisplayed.Add(count);

            

            return pagesDisplayed.Distinct().ToList();


        }


        private void SetArticleTags(Post post, string tags, ForumDbContext database)
        {
            if (tags != null)
            {
                var tagsString = tags.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLower()).Distinct();

                post.Tags.Clear();

                foreach (var tagString in tagsString)
                {
                    Tag tag = database.Tags.FirstOrDefault(x => x.Name.Equals(tagString));

                    if (tag == null)
                    {
                        tag = new Tag() { Name = tagString };
                        database.Tags.Add(tag);

                    }

                    post.Tags.Add(tag);
                }
            }


        }
        


    }
}