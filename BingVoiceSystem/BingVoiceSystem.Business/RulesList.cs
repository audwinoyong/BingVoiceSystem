using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.Business
{
    public class RulesList
    {
        public List<PendingRule> PendingRulesList;
        public List<ApprovedRule> ApprovedRulesList;
        public List<RejectedRule> RejectedRulesList;

        public RulesList()
        {
            BingDBEntities db = new BingDBEntities();

            PendingRulesList = db.PendingRules.ToList();
            ApprovedRulesList = db.ApprovedRules.ToList();
            RejectedRulesList = db.RejectedRules.ToList();
        }
    }
}
