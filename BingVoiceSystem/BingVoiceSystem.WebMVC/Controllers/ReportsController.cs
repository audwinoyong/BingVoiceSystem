using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BingVoiceSystem.Data;

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
            List<string> EditorsList = db.Database.SqlQuery<string>("SELECT Email from AspNetUsers WHERE Id IN (SELECT UserId from AspNetUserRoles WHERE RoleId = 'Editor')").ToList();
            List<List<string>> EditorData = new List<List<string>>();

            double AvgSuccessRate = 0;
            int Count = 0;

            foreach (string user in EditorsList)
            {
                double ApprovedRulesCount = db.ApprovedRules.Where(q => q.CreatedBy == user).ToList().Count;
                double RejectedRulesCount = db.RejectedRules.Where(q => q.CreatedBy == user).ToList().Count;
                double PendingRulesCount = db.PendingRules.Where(q => q.CreatedBy == user).ToList().Count;
                string SuccessRate = (ApprovedRulesCount / (ApprovedRulesCount + RejectedRulesCount) * 100).ToString("N0") + "%";

                if (ApprovedRulesCount == 0 && RejectedRulesCount == 0)
                {
                    SuccessRate = "-";
                }
                else
                {
                    AvgSuccessRate += int.Parse(SuccessRate.TrimEnd('%'));
                    Count++;
                }

                EditorData.Add(new List<string>
                {
                    user, ApprovedRulesCount.ToString("N0"), RejectedRulesCount.ToString("N0"), PendingRulesCount.ToString("N0"), SuccessRate
                });
            }

            ViewBag.ReportData = EditorData;
            ViewBag.AvgSuccessRate = AvgSuccessRate/Count + "%";
            return View();
        }

        public ActionResult EditorReport()
        {
            List<ApprovedRule> UsersApprovedRules = db.ApprovedRules.Where(q => q.CreatedBy == User.Identity.Name).ToList();

            double UsersApprovedRulesCount = db.ApprovedRules.Where(q => q.ApprovedBy == User.Identity.Name).ToList().Count;
            double UsersRejectedRulesCount = db.RejectedRules.Where(q => q.RejectedBy == User.Identity.Name).ToList().Count;

            ViewBag.UsersApprovedRules = UsersApprovedRules;
            ViewBag.UsersApprovedRulesCount = UsersApprovedRulesCount;
            ViewBag.UsersRejectedRulesCount = UsersRejectedRulesCount;
            ViewBag.ApprovalRate = (UsersApprovedRulesCount / (UsersApprovedRulesCount + UsersRejectedRulesCount) * 100).ToString("N0") + "%";
            return View();
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