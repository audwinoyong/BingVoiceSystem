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

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            GlobalState.rules.AddRule(QuestionTextBox.Text, AnswerTextBox.Text, User.Identity.GetUserId(), "PendingRules");
            Response.Redirect("~/RulesListEditor.aspx");
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RulesListEditor.aspx");
        }
    }
}