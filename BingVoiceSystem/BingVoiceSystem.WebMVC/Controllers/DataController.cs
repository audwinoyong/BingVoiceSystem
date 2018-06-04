using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;
using BingVoiceSystem.Business;
using System.Collections.Generic;
using System.Net;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class DataController : Controller
    {
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
            Business.Data data = new Business.Data();
            if (ModelState.IsValid)
            {
                if (data.AddData(model.MovieName, model.Genre, data.ActorsFromString(model.Actors), User.Identity.Name))
                {
                    return RedirectToAction("DataList");
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

        // GET: Rules/Edit/243?table=PendingRules
        // Show an edit form to edit an existing rule
        public ActionResult Edit(int? id, string table)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = data.S
            switch (table)
            {
                case "ApprovedRules":
                    var apprule = rules.SearchApprovedRule((int)id);
                    if (apprule == null)
                    {
                        return HttpNotFound();
                    }
                    return View(
                        new RulesModel
                        {
                            ApprovedRule = apprule
                        }
                    );
                case "RejectedRules":
                    var rejrule = rules.SearchRejectedRule((int)id);
                    if (rejrule == null)
                    {
                        return HttpNotFound();
                    }
                    return View(
                        new RulesModel
                        {
                            RejectedRule = rejrule
                        }
                    );
                case "PendingRules":
                    var penrule = rules.SearchPendingRule((int)id);
                    if (penrule == null)
                    {
                        return HttpNotFound();
                    }
                    return View(
                        new RulesModel
                        {
                            PendingRule = penrule
                        }
                    );
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown table");
                    return View();
            }
        }
    }
}