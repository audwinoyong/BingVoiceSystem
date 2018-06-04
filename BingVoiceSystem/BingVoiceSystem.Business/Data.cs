using BingVoiceSystem.Data;
//using BingVoiceSystem.WebMVC.Models;
using System.Linq;
using System.Collections.Generic;


namespace BingVoiceSystem.Business
{
    public class Data
    {
        public List<DataList> DataList;

        public Data()
        {
            BingDBEntities db = new BingDBEntities();
            DataList = new List<DataList>();

            List<string> Movies = db.Movies.Select(q => q.MovieName).ToList();

            foreach (string Movie in Movies)
            {
                string Genre = db.Genres.Where(q => q.MovieName == Movie).Select(q => q.GenreType).ToList()[0];
                List<string> Actors = db.Actors.Where(q => q.MovieName == Movie).Select(q => q.ActorName).ToList();

                DataList.Add(new DataList { Movie = Movie, Genre = Genre, Actors = Actors });
            }
        }

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
                db.Genres.Add(new Genre { GenreType = Genre, MovieName = MovieName });
                foreach (string Actor in Actors)
                {
                    db.Actors.Add(new Actor { MovieName = MovieName, ActorName = Actor });
                }
                db.SaveChanges();

                return true;
            }
        }
    }
}
