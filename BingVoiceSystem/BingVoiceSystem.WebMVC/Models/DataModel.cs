using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BingVoiceSystem.WebMVC.Models
{
    public class DataModel
    {
        public int MovieId { get; set; }

        public string MovieName { get; set; }

        public string Genre { get; set; }

        public string Actor { get; set; }

        public List<string> Actors { get; set; }
    }
}