using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BingVoiceSystem.Data;
using BingVoiceSystem.Business;

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
            ApproverReport report = new ApproverReport();
            return View(report);
        }

        public ActionResult EditorReport()
        {
            EditorReport report = new EditorReport(User.Identity.Name);
            return View(report);
        }

        public ActionResult RulesReport()
        {
            List<ApprovedRule> ApprovedRulesList = db.ApprovedRules.ToList();

            double ApprovedListCount = ApprovedRulesList.Count;
            double RejectedListCount = db.RejectedRules.ToList().Count;

            ViewBag.ApprovedRules = ApprovedRulesList;
            ViewBag.ApprovedCount = (int) ApprovedListCount;
            ViewBag.RejectedCount = (int) RejectedListCount;
            ViewBag.SuccessRate = (ApprovedListCount / (ApprovedListCount + RejectedListCount) * 100).ToString("N0") + "%";

            return View();
        }
    }
}