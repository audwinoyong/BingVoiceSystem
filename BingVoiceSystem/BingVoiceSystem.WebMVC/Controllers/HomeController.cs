using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BingVoiceSystem;
using BingVoiceSystem.WebMVC.Models;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            RulesModel model = new RulesModel();
            return View(model);
        }

        
        [HttpPost]
        public ActionResult Index([Bind(Include = "Question,Answer")]RulesModel model)
        {
            model.SetAnswer(model.Question);
            return View(model);
        }
        
        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}