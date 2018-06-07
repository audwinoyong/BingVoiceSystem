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
    /// <summary>
    /// The list of rules.
    /// </summary>
    [Authorize]
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
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult Add()
        {
            return View();
        }

        // POST: Rules/Add
        // Add a Pending Rule based on the supplied data
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult Add([Bind(Include = "Question,Answer,Lookup")] RulesModel model)
        {
            if (ModelState.IsValid)
            {
                string result;
                if ((result = rules.AddRule(model.Question, model.Answer, User.Identity.Name, User.Identity.Name, User.Identity.Name, model.Lookup, Table.PendingRules)) == null)
                {
                    return RedirectToAction("RulesList");
                }
                else
                {
                    ViewBag.DuplicateError = result;
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
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult Edit(int? id, Table table)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            switch (table)
            {
                case Table.ApprovedRules:
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
                case Table.RejectedRules:
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
                case Table.PendingRules:
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

        // POST: Rules/Edit/243?table=PendingRules
        // Save changes to an edited rule
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult Edit(RulesModel model, Table table)
        {
            string result = "";
            switch (table)
            {
                case Table.ApprovedRules:
                    result = rules.EditRule(model.ApprovedRule.RuleID, model.ApprovedRule.Question, model.ApprovedRule.Answer, User.Identity.Name, model.ApprovedRule.Lookup, Table.ApprovedRules);
                    break;
                case Table.RejectedRules:
                    result = rules.EditRule(model.RejectedRule.RuleID, model.RejectedRule.Question, model.RejectedRule.Answer, User.Identity.Name, model.RejectedRule.Lookup, Table.RejectedRules);
                    break;
                case Table.PendingRules:
                    result = rules.EditRule(model.PendingRule.RuleID, model.PendingRule.Question, model.PendingRule.Answer, User.Identity.Name, model.PendingRule.Lookup, Table.PendingRules);
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown table");
                    break;
            }

            if (result == null)
            {
                return RedirectToAction("RulesList");
            }
            else
            {
                ViewBag.DuplicateError = result;
                return View(model);
            }

        }

        // GET: Rules/Delete/243?table=PendingRules
        // Retrieve details of a rule to confirm deletion
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult Delete(int? id, Table table)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            switch (table)
            {
                case Table.ApprovedRules:
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
                case Table.RejectedRules:
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
                case Table.PendingRules:
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

        // POST: Rules/Delete/243?table=PendingRules
        // Delete a rule
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "DataMaintainer, Editor")]
        public ActionResult DeleteConfirmed(int id, Table table)
        {
            switch (table)
            {
                case Table.ApprovedRules:
                    var apprule = rules.SearchApprovedRule(id);
                    if (apprule == null)
                    {
                        return HttpNotFound();
                    }
                    rules.DeleteRule(apprule.Question, Table.ApprovedRules);
                    break;
                case Table.RejectedRules:
                    var rejrule = rules.SearchRejectedRule(id);
                    if (rejrule == null)
                    {
                        return HttpNotFound();
                    }
                    rules.DeleteRule(rejrule.Question, Table.RejectedRules);
                    break;
                case Table.PendingRules:
                    var penrule = rules.SearchPendingRule(id);
                    if (penrule == null)
                    {
                        return HttpNotFound();
                    }
                    rules.DeleteRule(penrule.Question, Table.PendingRules);
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown table");
                    return RedirectToAction("RulesList");
            }
            return RedirectToAction("RulesList");
        }

        // GET: Rules/Approve/243
        // Retrieve details of a rule to confirm approval
        [Authorize(Roles = "Approver")]
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
        [Authorize(Roles = "Approver")]
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
        [Authorize(Roles = "Approver")]
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
        [Authorize(Roles = "Approver")]
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