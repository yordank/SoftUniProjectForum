﻿using System;
using System.Collections.Generic;
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
            // return RedirectToAction("ListQuestions", "Post");
            return View();
        }
       

    }
}