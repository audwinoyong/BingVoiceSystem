using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BingVoiceSystem.Data;
using BingVoiceSystem.WebMVC.Models;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class RulesController : Controller
    {
        private BingDBEntities db = new BingDBEntities();

        private EFRules rules = new EFRules();

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        // GET: Rules/RulesList
        // Show the list of all rules
        public ActionResult RulesList()
        {
            List<PendingRule> PendingRulesList = db.PendingRules.ToList();
            List<ApprovedRule> ApprovedRulesList = db.ApprovedRules.ToList();
            List<RejectedRule> RejectedRulesList = db.RejectedRules.ToList();

            ViewBag.PendingRules = PendingRulesList;
            ViewBag.ApprovedRules = ApprovedRulesList;
            ViewBag.RejectedRules = RejectedRulesList;

            return View();
        }

        // GET: Rules/Add
        // Show an edit form to add a new rule
        public ActionResult Add()
        {
            return View();
        }

        // POST: Rules/Add
        // Add a Pending Rule based on the supplied data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Question,Answer")] RulesModel model)
        {
            if (ModelState.IsValid)
            {
                if (rules.AddRule(model.Question, model.Answer, User.Identity.Name, "PendingRules"))
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

        // GET: Rules/Edit/243?table=PendingRules
        // Show an edit form to edit an existing rule
        public ActionResult Edit(int? id, string table)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rule = rules.SearchPendingRule((int)id);
            if (rule == null)
            {
                return HttpNotFound();
            }
            return View(rule);
        }

        // POST: Rules/Edit/243?table=PendingRules
        // Save changes to an edited rule
        [HttpPost]
        public ActionResult Edit(RulesModel model)
        {
            rules.EditRule(model.RuleID, model.Question, model.Answer, User.Identity.Name, "PendingRules");
            return RedirectToAction("RulesList");
        }

        // GET: Rules/Delete/243?table=PendingRules
        // Retrieve details of a rule to confirm deletion
        public ActionResult Delete(int? id, string table)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rule = rules.SearchPendingRule((int)id);
            if (rule == null)
            {
                return HttpNotFound();
            }
            return View(rule);
        }

        // POST: Rules/Delete/243?table=PendingRules
        // Delete a rule
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var rule = rules.SearchPendingRule(id);
            if (rule == null)
            {
                return HttpNotFound();
            }
            rules.DeleteRule(rule.Question, "PendingRules");
            return RedirectToAction("RulesList");
        }

    }
}