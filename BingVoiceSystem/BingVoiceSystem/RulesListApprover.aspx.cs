using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class RulesListApprover : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowData();
            }
        }

        protected void ShowData()
        {
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

        // Only for approving or rejecting rule by Approver
        protected void PendingRulesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            string question = PendingRulesGridView.DataKeys[index].Value.ToString();

            if (e.CommandName.Equals("Approve"))
            {
                GlobalState.rules.ApproveRule(question, User.Identity.GetUserName());
            }
            else if (e.CommandName.Equals("Reject"))
            {
                GlobalState.rules.RejectRule(question, User.Identity.GetUserName());
            }
            ShowData();
        }
    }
}