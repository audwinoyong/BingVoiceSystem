using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;
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
        public ActionResult DataEdit(int MovieID)
        {
            if (MovieID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Business.Data data = new Business.Data();
                DataModel model = new DataModel();
                model.DataList = data.CreateDataList(MovieID);
                model.ActorString = data.ActorsToString(model.DataList.Actors);

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult DataEdit(DataModel model)
        {
            Business.Data data = new Business.Data();
            data.EditData(model.DataList.MovieID, model.DataList.MovieName, model.DataList.Genre, data.ActorsFromString(model.ActorString), User.Identity.Name);
            return RedirectToAction("DataList");
        }

        // GET: Data/DataDelete
        // Show an edit form to edit an existing data
        public ActionResult DataDelete(int MovieID)
        {
            if (MovieID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Business.Data data = new Business.Data();
                DataModel model = new DataModel();
                model.DataList = data.CreateDataList(MovieID);
                model.ActorString = data.ActorsToString(model.DataList.Actors);

                return View(model);
            }
        }

        // POST: Data/Delete/243?MovieID=7
        // Delete data
        [HttpPost, ActionName("DataDelete")]
        public ActionResult DeleteConfirmed(int MovieId)
        {
            Business.Data data = new Business.Data();
            var DataList = data.CreateDataList(MovieId);

            if (DataList == null)
            {
                return HttpNotFound();
            }
            else
            {
                data.DeleteData(MovieId);
                return RedirectToAction("DataList");
            }
        }
    }
}