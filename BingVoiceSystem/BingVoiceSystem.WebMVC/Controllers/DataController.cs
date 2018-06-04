using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;
using BingVoiceSystem.Business;
using System.Collections.Generic;
using System.Net;

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

        public ActionResult DataAdd()
        {
            return View();
        }

        // POST: Data/DataAdd
        // Add a Pending Rule based on the supplied data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddData([Bind(Include = "MovieName,Genre,Actors")] DataModel model)
        {
            Business.Data data = new Business.Data();
            if (ModelState.IsValid)
            {
                if (data.AddData(model.MovieName, model.Genre, data.ActorsFromString(model.Actors), User.Identity.Name))
                {
                    return RedirectToAction("DataList");
                }
                else
                {
                    ViewBag.DuplicateError = "That question already has an answer";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Data/DataEdit
        // Show an edit form to edit an existing data
        public ActionResult DataEdit(string MovieName)
        {
            if (MovieName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Business.Data data = new Business.Data();
                return View(new DataModel { DataList = data.SearchData(MovieName) });
            }
        }
    }
}