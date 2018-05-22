using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;

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
            return View();
        }

        public ActionResult EditorReport()
        {
            return View();
        }

        public ActionResult RulesReport()
        {
            return View();
        }
    }
}