using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class _Default : Page
    {
        Rules rules;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            rules = new Rules();
            
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            if (submitBtn.Text.Equals("Submit"))
            {
                answerLbl.Text = "Answer: " + rules.GetAnswer(questionTb.Text);
                answerLbl.Visible = true;
                submitBtn.Text = "New Question";                
            }
            else
            {
                questionTb.Text = "";
                answerLbl.Visible = false;
                submitBtn.Text = "Submit";
            }
            //line below is a test to get the user identity
            //answersTB.Text = System.Web.HttpContext.Current.User.Identity.Name;
        }
    }
}