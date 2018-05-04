using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class MovieRulesList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LookupDropDown.Items.Insert(0, new ListItem("Movie Title", "MovieTitle"));
            LookupDropDown.Items.Insert(1, new ListItem("Genre", "Genre"));
            AnswerDropDown.Items.Insert(0, new ListItem("Movie Title", "MovieTitle"));
            AnswerDropDown.Items.Insert(1, new ListItem("Genre", "Genre"));
        }

        /// <summary>
        /// Event to add the rule into the Rules table, and check if the rule has already exists in the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string DataDrivenQuestion = "";
            string DataDrivenResponse = "";

            if (QuestionTextBox.Text.Contains('%'))
            {
                foreach (Char c in QuestionTextBox.Text)
                {
                    if (c == '%')
                    {
                        DataDrivenQuestion += "[%]";
                    }
                    else DataDrivenQuestion += c;
                }
            }
            else
            {
                RuleAdded.Text = "The question requies a wild-card (%)";
            }

            if (ResponseTextBox.Text.Contains('%'))
            {
                foreach (Char c in QuestionTextBox.Text)
                {
                    if (c == '%')
                    {
                        DataDrivenResponse += "[%]";
                    }
                    else DataDrivenResponse += c;
                }
            }
            else
            {
                RuleAdded.Text = "The question requies a wild-card (%)";
            }

            if (GlobalState.rules.AddRule(DataDrivenQuestion, DataDrivenResponse + "|" + LookupDropDown.SelectedValue + "|" + AnswerDropDown.SelectedValue, User.Identity.GetUserName(), "PendingRules", true))
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