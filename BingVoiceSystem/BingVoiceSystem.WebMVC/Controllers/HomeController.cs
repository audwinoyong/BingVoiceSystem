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
            if (ModelState.IsValidField("Question"))
            {
                EFRules Rules = new EFRules();
                model.Answer = Rules.GetAnswer(model.Question);
            }
            else
            {
                ViewBag.EmptyError = "The Question field cannot be empty.";
            }
            return View(model);
        }
    }
}