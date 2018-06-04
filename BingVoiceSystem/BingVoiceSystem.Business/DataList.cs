using System.Collections.Generic;

namespace BingVoiceSystem.Business
{
    public class DataList
    {
        public int MovieID{ get; set; }
        public string MovieName { get; set; }
        public string Genre { get; set; }
        public List<string> Actors { get; set; }
        public string LastEditedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
