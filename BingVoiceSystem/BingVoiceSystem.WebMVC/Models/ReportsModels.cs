using System;
using System.Collections.Generic;
using System.Web;

namespace BingVoiceSystem.WebMVC.Models
{
    public class ReportsModels
    {
        public List<string> ApprovedRules { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}
