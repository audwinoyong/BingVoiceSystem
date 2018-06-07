using System.Collections.Generic;
using System.Linq;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.Business
{
    /// <summary>
    /// Business logic for the Rules Report.
    /// </summary>
    public class RulesReport
    {
        // A list of approved rules
        public List<ApprovedRule> ApprovedRulesList;

        // The number of approved rules
        public int ApprovedCount;

        // The number of rejected rules
        public int RejectedCount;

        // The percentage success rate
        public string SuccessRate;

        /// <summary>
        /// Constructor for the Rules Report.
        /// </summary>
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
