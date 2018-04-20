using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class EditorsReport : System.Web.UI.Page
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

            Dictionary<string, string> approvedRulesList = GlobalState.rules.PrintUserRules(User.Identity.Name);

            MyRulesGridView.DataSource = approvedRulesList;
            MyRulesGridView.DataBind();


        }
    }
}