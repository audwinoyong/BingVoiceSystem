﻿using System.Web.Mvc;
using BingVoiceSystem.Business;

namespace BingVoiceSystem.WebMVC.Controllers
{
    /// <summary>
    /// The reports to be generated.
    /// </summary>
    [Authorize]
    public class ReportsController : Controller
    {
        // GET: Reports/EditorReport
        // Shows the Editor's report
        [Authorize(Roles = "Editor")]
        public ActionResult EditorReport()
        {
            EditorReport report = new EditorReport(User.Identity.Name);
            return View(report);
        }

        // GET: Reports/RulesReport
        // Shows the Rules report
        [Authorize(Roles = "Approver")]
        public ActionResult RulesReport()
        {
            RulesReport report = new RulesReport();
            return View(report);
        }

        // GET: Reports/ApproverReport
        // Shows the Approver's report
        [Authorize(Roles = "Approver")]
        public ActionResult ApproverReport()
        {
            ApproverReport report = new ApproverReport();
            return View(report);
        }
    }
}