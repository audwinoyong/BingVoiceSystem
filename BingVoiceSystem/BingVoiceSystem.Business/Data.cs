using BingVoiceSystem.Data;
using System.Linq;
using System.Collections.Generic;


namespace BingVoiceSystem.Business
{
    public class Data
    {
        public List<List<string>> DataList;

        public Data()
        {
            BingDBEntities db = new BingDBEntities();

            DataList = db.Database.SqlQuery<List<string>>("SELECT Movies.Movie, Genres.Genre, Actors.Actor FROM Movies INNER JOIN Genres ON Genres.Movie = Movies.Movie INNER JOIN Actors ON Actors.Movie = Movies.Movie").ToList();
        }
    }
}
