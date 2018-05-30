using System.Collections.Generic;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    public class ReportsModels
    {
        //MUST ASK HOW TO INCORPORATE MODEL?
        public List<ApprovedRule> ApprovedRules { get; set; }

        public List<RejectedRule> RejectedRules { get; set; }
    }
}
