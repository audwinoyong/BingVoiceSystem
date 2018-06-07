using BingVoiceSystem.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BingVoiceSystem.Business
{
    /// <summary>
    /// Business logic for the Data.
    /// </summary>
    public class Data
    {
        // The list of Data
        public List<DataList> DataList;

        /// <summary>
        /// Constructor of the Data.
        /// </summary>
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

        /// <summary>
        /// Gets desired data from the data tables.
        /// </summary>
        /// <param name="LookupTable">Table to lookup the comparison for the Value</param>
        /// <param name="Value">The value to compare</param>
        /// <param name="AnswerTable">Table to get the relevant answers</param>
        /// <returns>The desired data</returns>
        public string GetData(string LookupTable, string Value, string AnswerTable)
        {
            using (var db = new BingDBEntities())
            {
                List<int> Matches = new List<int>();
                List<string> Answers = new List<string>();

                switch (LookupTable)
                {
                    case "Movies":
                        Matches = db.Movies.Where(q => q.MovieName.ToLower() == Value.ToLower()).Select(q => q.MovieID).ToList();
                        break;
                    case "Genres":
                        Matches = db.Genres.Where(q => q.GenreType.ToLower() == Value.ToLower()).Select(q => q.MovieID).ToList();
                        break;
                    case "Actors":
                        Matches = db.Actors.Where(q => q.ActorName.ToLower() == Value.ToLower()).Select(q => q.MovieID).ToList();
                        break;
                }

                switch (AnswerTable)
                {
                    case "{Movies}":
                        foreach (int Match in Matches)
                        {
                            Answers.AddRange(db.Movies.Where(q => q.MovieID == Match).Select(q => q.MovieName).ToList());
                        }
                        break;
                    case "{Genres}":
                        foreach (int Match in Matches)
                        {
                            Answers.AddRange(db.Genres.Where(q => q.MovieID == Match).Select(q => q.GenreType).ToList());
                        }
                        break;
                    case "{Actors}":
                        foreach (int Match in Matches)
                        {
                            Answers.AddRange(db.Actors.Where(q => q.MovieID == Match).Select(q => q.ActorName).ToList());
                        }
                        break;
                }

                if (Answers.Count == 0)
                {
                    return "Sorry, no data was found for that query";
                }
                else
                {
                    string answer = "";
                    foreach (string ans in Answers)
                    {
                        answer = answer + ans + ", ";
                    }
                    return answer.Substring(0, answer.Length - 2);
                }
            }
        }

        /// <summary>
        /// Add data to the relevant tables in the database.
        /// </summary>
        /// <param name="MovieName">The movie name</param>
        /// <param name="Genre">The genre</param>
        /// <param name="Actors">The actors</param>
        /// <param name="CreatedBy">User who created this data</param>
        /// <returns>Error message if adding data fails</returns>
        public string DataAdd(string MovieName, string Genre, List<string> Actors, string CreatedBy)
        {
            if (MovieName == null || Genre == null || Actors == null)
            {
                return "Movie, Genre and Actors fields are required.";
            }
            else if (CheckDuplicate(MovieName, -1) != null)
            {
                return (CheckDuplicate(MovieName, -1));
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

                return null;
            }
        }

        /// <summary>
        /// Returns true if the movie data trying to be added is a duplicate movie.
        /// </summary>
        /// <param name="Movie">The movie title</param>
        /// <param name="MovieID">The movie ID</param>
        /// <returns>Error message if duplicate exists</returns>
        public string CheckDuplicate(string Movie, int MovieID)
        {
            List<int> Matches = new List<int>();
            using (var db = new BingDBEntities())
            {

                Matches = db.Movies.Where(q => q.MovieName == Movie).Select(q => q.MovieID).ToList();
            }
            if (Matches.Count > 0)
            {
                int sameIDCount = 0;
                foreach (int match in Matches)
                {
                    if (match == MovieID) { sameIDCount++; }
                }
                if (sameIDCount > 0)
                {
                    return null;
                }
                else
                {
                    return "That movie already exists. Please edit the movie on the Data List screen.";
                }
            }
            return null;
        }


        /// <summary>
        /// Handles data edits. Updates movie and genre details, deletes all existing actors and adds a new list of actors.
        /// </summary>
        /// <param name="MovieID">The movie ID</param>
        /// <param name="MovieName">The movie name</param>
        /// <param name="Genre">The movie genre</param>
        /// <param name="Actors">The actors of the movie</param>
        /// <param name="LastEditedBy">The last User who edits the data</param>
        /// <returns>Error message if editing fails</returns>
        public string EditData(int MovieID, string MovieName, string Genre, List<string> Actors, string LastEditedBy)
        {
            //ADD CHECK TO SEE IF MOVIE NAME ALREADY EXISTS
            if (MovieName == null || Genre == null || Actors == null)
            {
                return "Movie, Genre and Actors fields are required.";
            }
            else if (CheckDuplicate(MovieName, MovieID) != null)
            {
                return (CheckDuplicate(MovieName, MovieID));
            }


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

            return null;
        }

        /// <summary>
        /// Delete the data
        /// </summary>
        /// <param name="MovieID">The movie ID</param>
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

        /// <summary>
        /// Splits a string into a List of values. Determines item by separation of ';'.
        /// </summary>
        /// <param name="actors">Name of the actors</param>
        /// <returns>The list of the actors</returns>
        public List<string> ActorsFromString(string actors)
        {
            if (actors == null)
            {
                return null;
            }
            List<string> Actors = new List<string>();
            string actor = "";
            for (int i = 0; i < actors.Length; i++)
            {
                if (i == actors.Length - 1 && !actors[i].Equals(';'))
                {
                    actor += actors[i];
                    Actors.Add(actor.Trim());
                }
                else if (!actors[i].Equals(';'))
                {
                    actor += actors[i];
                }
                else
                {
                    Actors.Add(actor.Trim());
                    actor = "";
                }
            }
            return Actors;
        }

        /// <summary>
        /// Create a complete DataList from a MovieID.
        /// </summary>
        /// <param name="MovieID">The movie ID</param>
        /// <returns>A data list of the Movie</returns>
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

        /// <summary>
        /// Get MovieID from the name of a movie.
        /// </summary>
        /// <param name="MovieName">Name of the movie</param>
        /// <returns>The ID of the movie</returns>
        public int MovieNameToID(string MovieName)
        {
            using (var db = new BingDBEntities())
            {
                int Movie = db.Movies.Where(q => q.MovieName == MovieName).Select(q => q.MovieID).First();
                return Movie;
            }
        }

        /// <summary>
        /// Convert actor list to a string for view.
        /// </summary>
        /// <param name="Actors">The list of actors</param>
        /// <returns>A string converted from the list</returns>
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
