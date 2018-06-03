using System.Collections.Generic;
using System.Linq;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.Business
{
    public class ApproverReport
    {
        public List<List<string>> EditorData;
        public string AvgSuccessRate; 

        public ApproverReport()
        {
            BingDBEntities db = new BingDBEntities();
            EditorData = new List<List<string>>();

            List<string> EditorsList = db.Database.SqlQuery<string>("SELECT Email from AspNetUsers WHERE Id IN (SELECT UserId from AspNetUserRoles WHERE RoleId = 'Editor')").ToList();
            double AvgSuccessRate = 0;
            int Count = 0;

            foreach (string user in EditorsList)
            {
                double ApprovedRulesCount = db.ApprovedRules.Where(q => q.CreatedBy == user).ToList().Count;
                double RejectedRulesCount = db.RejectedRules.Where(q => q.CreatedBy == user).ToList().Count;
                double PendingRulesCount = db.PendingRules.Where(q => q.CreatedBy == user).ToList().Count;
                string SuccessRate = (ApprovedRulesCount / (ApprovedRulesCount + RejectedRulesCount) * 100).ToString("N0") + "%";

                if (ApprovedRulesCount == 0 && RejectedRulesCount == 0)
                {
                    SuccessRate = "-";
                }
                else
                {
                    AvgSuccessRate += int.Parse(SuccessRate.TrimEnd('%'));
                    Count++;
                }

                EditorData.Add(new List<string>
                {
                    user, ApprovedRulesCount.ToString("N0"), RejectedRulesCount.ToString("N0"), PendingRulesCount.ToString("N0"), SuccessRate
                });
            }

            this.AvgSuccessRate = (AvgSuccessRate / Count).ToString("N0") + "%";
        }
    }
}
