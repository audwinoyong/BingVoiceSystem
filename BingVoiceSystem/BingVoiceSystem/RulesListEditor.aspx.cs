using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class RulesListEditor : System.Web.UI.Page
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
            string pendingRuleSqlCommand = "SELECT * FROM PendingRules";
            string approvedRuleSqlCommand = "SELECT * FROM ApprovedRules";
            string rejectedRuleSqlCommand = "SELECT * FROM RejectedRules";

            BindRules(pendingRuleSqlCommand, PendingRulesGridView);
            BindRules(approvedRuleSqlCommand, ApprovedRulesGridView);
            BindRules(rejectedRuleSqlCommand, RejectedRulesGridView);
        }

        protected void BindRules(string ruleSqlCommand, GridView gridView)
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(ruleSqlCommand, conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    gridView.DataSource = rdr;
                    gridView.DataBind();
                }
            }
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
            string oldquestion = PendingRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "PendingRules");
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

        // For Approved Rules GridView
        protected void ApprovedRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            ApprovedRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        protected void ApprovedRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldquestion = ApprovedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)ApprovedRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)ApprovedRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "ApprovedRules");
            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void ApprovedRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void ApprovedRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = ApprovedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "ApprovedRules");

            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }

        // For Rejected Rules GridView
        protected void RejectedRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            RejectedRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        protected void RejectedRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldquestion = RejectedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)RejectedRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)RejectedRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "RejectedRules");
            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void RejectedRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }

        protected void RejectedRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = RejectedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "RejectedRules");

            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }

    }
}