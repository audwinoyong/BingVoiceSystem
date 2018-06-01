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

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "Question,Answer")] RulesModel model)
        {
            if (ModelState.IsValid)
            {
                model.AddRule(model.Question, model.Answer, User.Identity.Name);
                return RedirectToAction("RulesList");
            }
            else
            {
                return View();
            }
        }
    }
}