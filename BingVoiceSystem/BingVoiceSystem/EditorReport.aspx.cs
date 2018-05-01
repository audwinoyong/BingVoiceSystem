using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class EditorReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        // Binds data to tables in Editor Report screen
        protected void ShowData()
        {
            double approvedCount = GlobalState.rules.PrintUsersApprovedRules(User.Identity.GetUserId()).Count();
            double rejectedCount = GlobalState.rules.PrintUsersRejectedRules(User.Identity.GetUserId()).Count();

            Dictionary<string, string> approvedRulesList = GlobalState.rules.PrintUsersApprovedRules(User.Identity.GetUserId());
            Dictionary<string, string> ruleStatistics = new Dictionary<string, string> {
            { "Count of Approved", approvedCount.ToString("N0") },
            { "Cound of Rejected", rejectedCount.ToString("N0") },
            { "Success Rate", (approvedCount / (approvedCount + rejectedCount) * 100).ToString("N0") + "%" }};

            MyRulesGridView.DataSource = approvedRulesList;
            MyRulesGridView.DataBind();

            RulesStatisticsGridView.DataSource = ruleStatistics;
            RulesStatisticsGridView.DataBind();
        }
    }
}