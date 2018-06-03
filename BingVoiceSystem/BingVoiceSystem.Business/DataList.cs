using System.Collections.Generic;

namespace BingVoiceSystem.Business
{
    public class DataList
    {
        public int MovieId { get; set; }
        public string Movie { get; set; }
        public string Genre { get; set; }
        public List<string> Actors { get; set; }
    }
}
