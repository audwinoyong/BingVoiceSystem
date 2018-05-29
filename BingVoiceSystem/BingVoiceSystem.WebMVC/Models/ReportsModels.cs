using System;
using System.Collections.Generic;
using System.Web;

namespace BingVoiceSystem.WebMVC.Models
{
    public class ReportsModels
    {

        //MUST ASK HOW TO INCORPORATE MODEL?
        public List<ApprovedRule> ApprovedRules { get; set; }

        public List<RejectedRule> RejectedRules { get; set; }
    }
}
