using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Editor") && !User.IsInRole("Approver"))
            {
                permissionsLbl.Visible = true;
            }
            else if (User.IsInRole("Editor"))
            {
                editorReportBtn.Visible = true;
            }
            else if (User.IsInRole("Approver"))
            {
                rulesReportBtn.Visible = true;
                approverReportBtn.Visible = true;
            }
        }

        protected void RulesReportBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("RulesReport.aspx");
        }

        protected void EditorReportBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditorReport.aspx");
        }

        protected void ApproverReportBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("ApproverReport.aspx");
        }
    }
}