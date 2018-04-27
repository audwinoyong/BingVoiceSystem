using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class RulesReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        protected void ShowData()
        {
            double approvedCount = GlobalState.rules.CountApproved();
            double rejectedCount = GlobalState.rules.CountRejected();

            Dictionary<string, string> approvedRulesList = GlobalState.rules.PrintApprovedRules();

            Dictionary<string, string> ruleStatistics = new Dictionary<string, string>
            {
                { "Count of Approved", approvedCount.ToString("N0") },
                { "Count of Rejected", rejectedCount.ToString("N0") },
                { "Success Rate", (approvedCount / (approvedCount + rejectedCount) * 100).ToString("N0") + "%" }
            };


            ApprovedRulesGridView.DataSource = approvedRulesList;
            ApprovedRulesGridView.DataBind();

            RulesStatisticsGridView.DataSource = ruleStatistics;
            RulesStatisticsGridView.DataBind();
        }
    }
}