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
            chat.InnerHtml += "<p align = 'right'> <font color='blue'> Question: </font>" + questionTb.Text + "</p>" + "<br />";
            chat.InnerHtml += "<p align = 'left'> <font color='red'> Question: </font>" + rules.GetAnswer(questionTb.Text) + "\r\n";           
        }
    }
}