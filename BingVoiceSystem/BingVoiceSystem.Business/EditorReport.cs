using BingVoiceSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingVoiceSystem.Business
{
    public class EditorReport
    {
        public List<ApprovedRule> UsersApprovedRules;
        public double UsersApprovedRulesCount;
        public double UsersRejectedRulesCount;
        public string ApprovalRate;

        public EditorReport(string Email)
        {
            BingDBEntities db = new BingDBEntities();

            UsersApprovedRules = db.ApprovedRules.Where(q => q.CreatedBy == Email).ToList();
            UsersApprovedRulesCount = db.ApprovedRules.Where(q => q.ApprovedBy == Email).ToList().Count;
            UsersRejectedRulesCount = db.RejectedRules.Where(q => q.RejectedBy == Email).ToList().Count;
            ApprovalRate = (UsersApprovedRulesCount / (UsersApprovedRulesCount + UsersRejectedRulesCount) * 100).ToString("N0") + "%";
        }
    }
}
