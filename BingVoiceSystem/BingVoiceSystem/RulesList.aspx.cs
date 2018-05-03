using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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
                // Load data only on the initial page request
                // On postback, load the data after deleting rows
                ShowData();
                UserColumnVisibility();
            }
        }

        /// <summary>
        /// Binds the GridViews with data from the database.
        /// </summary>
        protected void ShowData()
        {
            PendingRulesGridView.DataSource = GlobalState.rules.PrintPendingRules();
            PendingRulesGridView.DataBind();

            ApprovedRulesGridView.DataSource = GlobalState.rules.PrintApprovedRules();
            ApprovedRulesGridView.DataBind();

            RejectedRulesGridView.DataSource = GlobalState.rules.PrintRejectedRules();
            RejectedRulesGridView.DataBind();
        }

        /// <summary>
        /// Hides or Shows columns depending on whether the user should have access to it
        /// </summary>
        protected void UserColumnVisibility()
        {
            // Hide the Edit and Delete columns, also hide AddRule button
            if (!(User.IsInRole("Editor") || User.IsInRole("DataMaintainer")))
            {
                PendingRulesGridView.Columns[3].Visible = false;
                PendingRulesGridView.Columns[4].Visible = false;

                ApprovedRulesGridView.Columns[4].Visible = false;
                ApprovedRulesGridView.Columns[5].Visible = false;

                RejectedRulesGridView.Columns[4].Visible = false;
                RejectedRulesGridView.Columns[5].Visible = false;

                AddRule.Visible = false;
            }
            // Hide the Approve and Reject columns
            if (!User.IsInRole("Approver"))
            {
                PendingRulesGridView.Columns[5].Visible = false;
            }
        }

        /// <summary>
        /// Redirect to Add Rule screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddRuleButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RulesListAdd.aspx");
        }

        /// <summary>
        /// Redirect to Add Movie Rule screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddMovieRuleButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DataRulesListAdd.aspx");
        }

        /// <summary>
        /// Event to enter row editing mode for specific rule in the Pending Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PendingRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            PendingRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        /// <summary>
        /// Event to update the rows (rules) edited in the Pending Rules GridView to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PendingRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldquestion = PendingRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)PendingRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "PendingRules");
            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to cancel row editing mode in the Pending Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PendingRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to delete a row (rule) in the Pending Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PendingRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = PendingRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "PendingRules");

            PendingRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event for Approver Role to approve or reject a specific rule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PendingRulesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Approve") || e.CommandName.Equals("Reject"))
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


        /// <summary>
        /// Event to enter row editing mode for specific rule in the Approved Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApprovedRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            ApprovedRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        /// <summary>
        /// Event to update the rows (rules) edited in the Approved Rules GridView to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApprovedRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldquestion = ApprovedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)ApprovedRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)ApprovedRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "ApprovedRules");
            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to cancel row editing mode in the Approved Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApprovedRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to delete a row (rule) in the Approved Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ApprovedRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = ApprovedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "ApprovedRules");

            ApprovedRulesGridView.EditIndex = -1;
            ShowData();
        }


        /// <summary>
        /// Event to enter row editing mode for specific rule in the Rejected Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RejectedRulesGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // NewEditIndex property is used to determine the index of the row being edited.
            RejectedRulesGridView.EditIndex = e.NewEditIndex;
            ShowData();
        }

        /// <summary>
        /// Event to update the rows (rules) edited in the Rejected Rules GridView to the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RejectedRulesGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string oldquestion = RejectedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            TextBox QuestionTextBox = (TextBox)RejectedRulesGridView.Rows[e.RowIndex].Cells[0].Controls[0];
            TextBox AnswerTextBox = (TextBox)RejectedRulesGridView.Rows[e.RowIndex].Cells[1].Controls[0];

            GlobalState.rules.EditRule(oldquestion, QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "RejectedRules");
            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to cancel row editing mode in the Rejected Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RejectedRulesGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Setting the EditIndex property to -1 to cancel the Edit mode in Gridview  
            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }

        /// <summary>
        /// Event to delete a row (rule) in the Rejected Rules GridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RejectedRulesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string question = RejectedRulesGridView.DataKeys[e.RowIndex].Value.ToString();
            GlobalState.rules.DeleteRule(question, "RejectedRules");

            RejectedRulesGridView.EditIndex = -1;
            ShowData();
        }
    }
}