using BingVoiceSystem.Data;
using System.Collections.Generic;
using System.Linq;

namespace BingVoiceSystem.Business
{
    /// <summary>
    /// Business logic for the Editor's Report.
    /// </summary>
    public class EditorReport
    {
        // The list of approved rules by the current Editor
        public List<ApprovedRule> UsersApprovedRules;

        // The total count of approved rules the current Editor is responsible for
        public double UsersApprovedRulesCount;

        // The total count of rejected rules the current Editor is responsible for
        public double UsersRejectedRulesCount;

        // The percentage success rate of approval
        public string ApprovalRate;

        /// <summary>
        /// Constructor for the Editor's Report.
        /// </summary>
        /// <param name="Email">The email of the current Editor</param>
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
