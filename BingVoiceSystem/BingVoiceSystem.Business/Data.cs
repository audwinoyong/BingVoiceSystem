using BingVoiceSystem.Data;
//using BingVoiceSystem.WebMVC.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BingVoiceSystem.Business
{
    public class Data
    {
        public List<DataList> DataList;

        public Data()
        {
            BingDBEntities db = new BingDBEntities();
            DataList = new List<DataList>();

            List<int> Movies = db.Movies.Select(q => q.MovieID).ToList();

            foreach (int Movie in Movies)
            {
                var MovieData = db.Movies.Where(q => q.MovieID == Movie).First();
                string Genre = db.Genres.Where(q => q.MovieID == Movie).Select(q => q.GenreType).ToList().First();
                List<string> Actors = db.Actors.Where(q => q.MovieID == Movie).Select(q => q.ActorName).ToList();

                DataList.Add(new DataList { MovieID = Movie, MovieName = MovieData.MovieName, Genre = Genre, Actors = Actors, LastEditedBy = MovieData.LastEditedBy, CreatedBy = MovieData.CreatedBy });
            }
        }

        //Add data to the relevant tables in the database.
        public bool AddData(string MovieName, string Genre, List<string> Actors, string CreatedBy)
        {
            if (MovieName.Equals("") || Genre.Equals("") || Actors.Count == 0)
            {
                return false;
            }
            else
            {
                BingDBEntities db = new BingDBEntities();

                db.Movies.Add(new Movies { MovieName = MovieName, CreatedBy = CreatedBy });
                db.SaveChanges();

                db.Genres.Add(new Genre { GenreType = Genre, MovieID = MovieNameToID(MovieName) });
                foreach (string Actor in Actors)
                {
                    db.Actors.Add(new Actor { MovieID = MovieNameToID(MovieName), ActorName = Actor });
                }
                db.SaveChanges();

                return true;
            }
        }

        //Handles data edits. Updates movie and genre details, deletes all existing actors and adds a new list of actors.
        public bool EditData(int MovieID, string MovieName, string Genre, List<string> Actors, string LastEditedBy)
        {
            //ADD CHECK TO SEE IF MOVIE NAME ALREADY EXISTS

            using (var db = new BingDBEntities())
            {
                var MovieDB = db.Movies.Where(q => q.MovieID == MovieID).First();
                var GenreDB = db.Genres.Where(q => q.MovieID == MovieID).First();
                var ActorDB = db.Actors.Where(q => q.MovieID == MovieID).ToList();

                MovieDB.MovieName = MovieName;
                MovieDB.LastEditedBy = LastEditedBy;
                GenreDB.GenreType = Genre;

                foreach (Actor Actor in ActorDB)
                {
                    db.Actors.Remove(Actor);
                }
                db.SaveChanges();

                foreach (string Actor in Actors)
                {
                    db.Actors.Add(new Actor { MovieID = MovieID, ActorName = Actor });
                }
                db.SaveChanges();
            }

            return true;
        }

        public void DeleteData(int MovieID)
        {
            BingDBEntities db = new BingDBEntities();

            var MovieDB = db.Movies.Where(q => q.MovieID == MovieID).First();
            var GenreDB = db.Genres.Where(q => q.MovieID == MovieID).First();
            var ActorDB = db.Actors.Where(q => q.MovieID == MovieID).ToList();

            db.Movies.Remove(MovieDB);
            db.Genres.Remove(GenreDB);
            foreach (Actor Actor in ActorDB)
            {
                db.Actors.Remove(Actor);
            }
            db.SaveChanges();
        }

        //Splits a string into a List of values. Determines item by separation of ';'.
        public List<string> ActorsFromString(string actors)
        {
            List<string> Actors = new List<string>();
            string actor = "";
            for (int i = 0; i < actors.Length; i++)
            {
                if (i == actors.Length - 1 && !actors[i].Equals(';'))
                {
                    actor += actors[i];
                    Actors.Add(actor);
                }
                else if (!actors[i].Equals(';'))
                {
                    actor += actors[i];
                }
                else
                {
                    Actors.Add(actor);
                    actor = "";
                }
            }
            return Actors;
        }

        //Create a complete DataList from a MovieID.
        public DataList CreateDataList(int MovieID)
        {
            string MovieName;
            string Genre;
            List<string> Actors;

            using (var db = new BingDBEntities())
            {
                MovieName = db.Movies.Where(q => q.MovieID == MovieID).Select(q => q.MovieName).First();
                Genre = db.Genres.Where(q => q.MovieID == MovieID).Select(q => q.GenreType).ToList()[0];
                Actors = db.Actors.Where(q => q.MovieID == MovieID).Select(q => q.ActorName).ToList();
            }

            return new DataList { MovieID = MovieID, MovieName = MovieName, Genre = Genre, Actors = Actors };
        }

        //Get MovieID from the name of a movie.
        public int MovieNameToID(string MovieName)
        {
            using (var db = new BingDBEntities())
            {
                int Movie = db.Movies.Where(q => q.MovieName == MovieName).Select(q => q.MovieID).First();
                return Movie;
            }
        }

        //Convert actor list to a string for view.
        public string ActorsToString(List<string> Actors)
        {
            string ActorText = "";

            foreach (string Actor in Actors)
            {
                ActorText += Actor + ";";
            }

            return ActorText;
        }
    }
}
