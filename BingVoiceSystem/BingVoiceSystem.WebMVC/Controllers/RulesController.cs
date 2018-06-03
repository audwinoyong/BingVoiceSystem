using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BingVoiceSystem.Data;
using BingVoiceSystem.WebMVC.Models;
using BingVoiceSystem.Business;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class RulesController : Controller
    {
        private EFRules rules = new EFRules();

        // GET: Rules/RulesList
        // Show the list of all rules
        public ActionResult RulesList()
        {
            RulesList rulesList = new RulesList();
            return View(rulesList);
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
                if (rules.AddRule(model.Question, model.Answer, User.Identity.Name, User.Identity.Name, User.Identity.Name, "PendingRules"))
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

            //var rule = rules.SearchPendingRule((int)id);
            //if (rule == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(rule);
        }

        // POST: Rules/Edit/243?table=PendingRules
        // Save changes to an edited rule
        [HttpPost]
        public ActionResult Edit(RulesModel model, string table)
        {
            switch (table)
            {
                case "ApprovedRules":
                    rules.EditRule(model.ApprovedRule.RuleID, model.ApprovedRule.Question, model.ApprovedRule.Answer, User.Identity.Name, "ApprovedRules");
                    break;
                case "RejectedRules":
                    rules.EditRule(model.RejectedRule.RuleID, model.RejectedRule.Question, model.RejectedRule.Answer, User.Identity.Name, "RejectedRules");
                    break;
                case "PendingRules":
                    rules.EditRule(model.PendingRule.RuleID, model.PendingRule.Question, model.PendingRule.Answer, User.Identity.Name, "PendingRules");
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown table");
                    break;
            }
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

        // GET: Rules/Approve/243
        // Retrieve details of a rule to confirm approval
        public ActionResult Approve(int? id)
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

        // POST: Rules/Approve/243
        // Approve a rule
        [HttpPost, ActionName("Approve")]
        public ActionResult ApproveConfirmed(int id)
        {
            var rule = rules.SearchPendingRule(id);
            if (rule == null)
            {
                return HttpNotFound();
            }
            rules.ApproveRule(rule.Question, User.Identity.Name, rule.CreatedBy, rule.LastEditedBy);
            return RedirectToAction("RulesList");
        }

        // GET: Rules/Reject/243
        // Retrieve details of a rule to confirm rejection
        public ActionResult Reject(int? id)
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

        // POST: Rules/Reject/243
        // Reject a rule
        [HttpPost, ActionName("Reject")]
        public ActionResult RejectConfirmed(int id)
        {
            var rule = rules.SearchPendingRule(id);
            if (rule == null)
            {
                return HttpNotFound();
            }
            rules.RejectRule(rule.Question, User.Identity.Name, rule.CreatedBy, rule.LastEditedBy);
            return RedirectToAction("RulesList");
        }
    }
}