using System.Collections.Generic;
using System.Linq;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.Business
{
    public class RulesReport
    {
        public List<ApprovedRule> ApprovedRulesList;
        public int ApprovedCount;
        public int RejectedCount;
        public string SuccessRate;

        public RulesReport()
        {
            BingDBEntities db = new BingDBEntities();

            ApprovedRulesList = db.ApprovedRules.ToList();
            ApprovedCount = ApprovedRulesList.Count;
            RejectedCount = db.RejectedRules.ToList().Count;

            if (ApprovedCount == 0 && RejectedCount == 0)
            {
                SuccessRate = "-";
            }
            else SuccessRate = ((double)ApprovedCount / ((double)ApprovedCount + (double)RejectedCount) * 100).ToString("N0") + "%";          
        }
    }
}
