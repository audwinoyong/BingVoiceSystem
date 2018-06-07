using System.Web.Mvc;
using BingVoiceSystem.WebMVC.Models;
using System.Net;

namespace BingVoiceSystem.WebMVC.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        // GET: Data/DataList
        // Show the list of all data
        public ActionResult DataList()
        {
            Business.Data data = new Business.Data();
            return View(data);
        }

        // GET: Data/DataAdd
        // Show an edit form to add new data
        [Authorize(Roles = "DataMaintainer")]
        public ActionResult DataAdd()
        {
            return View();
        }

        // POST: Data/DataAdd
        // Add data based on the supplied data inputs
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DataMaintainer")]
        public ActionResult DataAdd([Bind(Include = "MovieName,Genre,Actors")] DataModel model)
        {
            Business.Data data = new Business.Data();
            if (ModelState.IsValid)
            {
                if (data.DataAdd(model.MovieName, model.Genre, data.ActorsFromString(model.Actors), User.Identity.Name))
                {
                    return RedirectToAction("DataList");
                }
                else
                {
                    ViewBag.DuplicateError = "That movie already exists. Please edit the movie on the Data List screen.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: Data/DataEdit?MovieID=15
        // Show an edit form to edit an existing data
        [Authorize(Roles = "DataMaintainer")]
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

        // POST: Data/DataEdit?MovieID=15
        // Save changes to an edited data
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DataMaintainer")]
        public ActionResult DataEdit(DataModel model)
        {
            Business.Data data = new Business.Data();
            data.EditData(model.DataList.MovieID, model.DataList.MovieName, model.DataList.Genre, data.ActorsFromString(model.ActorString), User.Identity.Name);
            return RedirectToAction("DataList");
        }

        // GET: Data/DataDelete?MovieID=15
        // Show an edit form to edit an existing data
        [Authorize(Roles = "DataMaintainer")]
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
        [Authorize(Roles = "DataMaintainer")]
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