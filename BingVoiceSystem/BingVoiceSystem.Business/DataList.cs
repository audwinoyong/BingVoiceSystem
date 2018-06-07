using System.Collections.Generic;

namespace BingVoiceSystem.Business
{
    /// <summary>
    /// Business logic for the list of data
    /// </summary>
    public class DataList
    {
        // The movie ID
        public int MovieID { get; set; }

        // The movie name
        public string MovieName { get; set; }

        // The genre of the movie
        public string Genre { get; set; }

        // The actors of the movie
        public List<string> Actors { get; set; }

        // The last User who edited the data
        public string LastEditedBy { get; set; }

        // The User who created the data
        public string CreatedBy { get; set; }
    }
}
