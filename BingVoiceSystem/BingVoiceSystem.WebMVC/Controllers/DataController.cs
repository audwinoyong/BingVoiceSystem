﻿using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;
using BingVoiceSystem.Business;
using System.Collections.Generic;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class DataController : Controller
    {
        private Business.Data data = new Business.Data();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DataList()
        {
            Business.Data data = new Business.Data();
            return View(data);
        }

        public ActionResult DataAdd()
        {
            return View();
        }

        public ActionResult ActorAdd()
        {
            return View();
        }

        // POST: Data/DataAdd
        // Add a Pending Rule based on the supplied data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddData([Bind(Include = "MovieName,Genre,Actors")] DataModel model)
        {
            if (ModelState.IsValid)
            {
                if (data.AddData(model.MovieName, model.Genre, GetActors(), User.Identity.Name))
                {
                    return RedirectToAction("RulesList");
                }
                else
                {
                    ViewBag.DuplicateError = "That question already has an answer";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public List<string> GetActors()
        {
            List<string> actors = new List<string>();

            return actors;
        }
    }
}