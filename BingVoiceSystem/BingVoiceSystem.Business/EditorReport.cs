using BingVoiceSystem.Data;
using System.Collections.Generic;
using System.Linq;

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

            if (UsersApprovedRulesCount == 0 && UsersRejectedRulesCount == 0)
            {
                ApprovalRate = "-";
            }
            else ApprovalRate = (UsersApprovedRulesCount / (UsersApprovedRulesCount + UsersRejectedRulesCount) * 100).ToString("N0") + "%";
        }
    }
}
