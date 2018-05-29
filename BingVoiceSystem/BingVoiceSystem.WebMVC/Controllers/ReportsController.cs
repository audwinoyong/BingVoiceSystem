using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class ReportsController : Controller
    {
        private BingDBEntities db = new BingDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApproverReport()
        {
            return View();
        }

        public ActionResult EditorReport()
        {
            return View();
        }

        public ActionResult RulesReport()
        {
            List<ApprovedRule> ApprovedRulesList = db.ApprovedRules.ToList();
            List<RejectedRule> RejectedRulesList = db.RejectedRules.ToList();

            double ApprovedListCount = ApprovedRulesList.Count;
            double RejectedListCount = RejectedRulesList.Count;

            ViewBag.ApprovedRules = ApprovedRulesList;
            ViewBag.RejectedRules = RejectedRulesList;
            ViewBag.ApprovedCount = (int) ApprovedListCount;
            ViewBag.RejectedCount = (int) RejectedListCount;
            ViewBag.SuccessRate = (ApprovedListCount / (ApprovedListCount + RejectedListCount) * 100).ToString("N0") + "%";

            return View();
        }
    }
}