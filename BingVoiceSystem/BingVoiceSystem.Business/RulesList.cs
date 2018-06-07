using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.Business
{
    /// <summary>
    /// Business logic for the list of rules.
    /// </summary>
    public class RulesList
    {
        // The list of pending rules
        public List<PendingRule> PendingRulesList;

        // The list of approved rules
        public List<ApprovedRule> ApprovedRulesList;

        // The list of rejected rules
        public List<RejectedRule> RejectedRulesList;

        /// <summary>
        /// Constructor for the RulesList.
        /// </summary>
        public RulesList()
        {
            BingDBEntities db = new BingDBEntities();

            PendingRulesList = db.PendingRules.ToList();
            ApprovedRulesList = db.ApprovedRules.ToList();
            RejectedRulesList = db.RejectedRules.ToList();
        }
    }
}
