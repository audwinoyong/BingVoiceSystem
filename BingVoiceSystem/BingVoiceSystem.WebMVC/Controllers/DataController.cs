using System.Web.Mvc;

namespace BingVoiceSystem.WebMVC.Controllers
{
    public class DataController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DataList()
        {
            Business.Data data = new Business.Data();
            return View(data);
        }
    }
}