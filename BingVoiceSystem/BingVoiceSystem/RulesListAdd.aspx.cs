using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class RulesListAdd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event to add the rule into the Rules table, and check if the rule has already exists in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (GlobalState.rules.AddRule(QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserName(), "PendingRules"))
            {
                RuleAdded.Text = "";
                Response.Redirect("~/RulesList.aspx");
            }
            else
            {
                RuleAdded.Text = "That question already has an answer";
            }
            
        }

        /// <summary>
        /// Event for cancelling, redirect back to Rules List screen page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RulesList.aspx");
        }
    }
}