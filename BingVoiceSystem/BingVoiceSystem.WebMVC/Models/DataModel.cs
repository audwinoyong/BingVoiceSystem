using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BingVoiceSystem.Business;

namespace BingVoiceSystem.WebMVC.Models
{
    /// <summary>
    /// The data model for data-driven rules.
    /// </summary>
    public class DataModel
    {
        // The movie ID
        public int MovieID { get; set; }

        // The movie name
        public string MovieName { get; set; }

        // The movie genre
        public string Genre { get; set; }

        // The actors of the movie
        public string Actors { get; set; }

        // The list of data
        public DataList DataList { get; set; }

        // The actors' string entered by User
        public string ActorString { get; set; }
    }
}