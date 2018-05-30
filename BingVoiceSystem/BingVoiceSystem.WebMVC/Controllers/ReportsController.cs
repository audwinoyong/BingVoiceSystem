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