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
    public partial class RulesListApprover : System.Web.UI.Page
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
    }
}