using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class RulesList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // load data on initial page request
                // on postback, load the data after deleting the rows

                //Rules rules = new Rules();
                Dictionary<string, string> pendingRulesList = GlobalState.rules.PrintPendingRules();
                Dictionary<string, string> approvedRulesList = GlobalState.rules.PrintApprovedRules();
                Dictionary<string, string> rejectedRulesList = GlobalState.rules.PrintRejectedRules();

                PendingRulesGridView.DataSource = pendingRulesList;
                PendingRulesGridView.DataBind();

                ApprovedRulesGridView.DataSource = approvedRulesList;
                ApprovedRulesGridView.DataBind();

                RejectedRulesGridView.DataSource = rejectedRulesList;
                RejectedRulesGridView.DataBind();
            }
        }

        protected void AddRuleButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RulesListEdit.aspx");
        }

    }
}