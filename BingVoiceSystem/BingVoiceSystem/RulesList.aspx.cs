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

        protected void AddRuleButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RulesListAdd.aspx");
        }

        protected void PendingRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            PendingRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        protected void PendingRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox QuestionTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.Name, "PendingRules");
            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void PendingRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void PendingRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = PendingRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "PendingRules");

            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        // for approving or rejecting rule
        protected void PendingRulesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Convert the row index stored in the CommandArgument property to an Integer.
            int index = Convert.ToInt32(e.CommandArgument);
            string question = PendingRulesGridView.DataKeys[index].Value.ToString();

            if (e.CommandName.Equals("Approve"))
            {
                GlobalState.rules.ApproveRule(question, User.Identity.Name);
            }
            else if (e.CommandName.Equals("Reject"))
            {
                GlobalState.rules.RejectRule(question, User.Identity.Name);
            }
            ShowData();
        }
    }
}