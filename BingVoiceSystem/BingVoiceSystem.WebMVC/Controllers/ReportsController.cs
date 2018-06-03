using System.Web.Mvc;
using BingVoiceSystem.Business;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class ReportsController : Controller
    {
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
            RulesReport report = new RulesReport();
            return View(report);
        }
    }
}