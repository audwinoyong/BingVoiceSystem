using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BingVoiceSystem.Business;

namespace BingVoiceSystem.WebMVC.Models
{
    public class DataModel
    {
        public int MovieId { get; set; }

        public string MovieName { get; set; }

        public string Genre { get; set; }

        public string Actors { get; set; }

        public DataList DataList { get; set; }
    }
}