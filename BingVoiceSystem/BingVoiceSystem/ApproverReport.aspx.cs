using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class ApproverReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        protected void ShowData()
        {
            List<string> users = GlobalState.user.GetUsersByRole(Role.Editor);

            Dictionary<string, string> editorStatistics = new Dictionary<string, string>();

            foreach (string user in users)
            {
                Console.WriteLine(user);

                double approvedCount = GlobalState.rules.PrintUsersApprovedRules(user).Count();
                double rejectedCount = GlobalState.rules.PrintUsersRejectedRules(user).Count();
                double pendingCount = GlobalState.rules.PrintUsersPendingRules(user).Count();

                List<string> values = new List<string>() {
                    approvedCount.ToString("N0"),
                    rejectedCount.ToString("N0"),
                    pendingCount.ToString("N0"),
                    (approvedCount / (approvedCount + rejectedCount) * 100).ToString("N0") + "%"
                };

                editorStatistics.Add(Membership.GetUser(user).UserName, approvedCount.ToString("N0"));
            }

            EditorStatisticsGridView.DataSource = editorStatistics;
            EditorStatisticsGridView.DataBind();
        }
    }
}