﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BingVoiceSystem
{
    public partial class ApproverReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        protected void ShowData()
        {
            int count = 0;
            double totalSuccess = 0;

            List<string> users = GlobalState.user.GetUsersByRole(Role.Editor);
            List<string> editorStatistics = new List<string>();

            // Since data is calculated from multiple sources and is more than two columns, 
            // manually creates a table and defines the column names.
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add("Editor", typeof(string));
            dt.Columns.Add("Approved Count", typeof(string));
            dt.Columns.Add("Rejected Count", typeof(string));
            dt.Columns.Add("Pending Count", typeof(string));
            dt.Columns.Add("Success Rate", typeof(string));
            dt.Columns.Add("Average Success Rate", typeof(string));

            //Fills in the data for each user of Editor role, also adds up all success rates for average calculation
            foreach (string user in users)
            {
                double approvedCount = GlobalState.rules.PrintUsersApprovedRules(user).Count();
                double rejectedCount = GlobalState.rules.PrintUsersRejectedRules(user).Count();
                double successRate = approvedCount / (approvedCount + rejectedCount) * 100;           
                             
                dr = dt.NewRow();
                dr["Editor"] = GlobalState.user.ConvertIdToUsername(user);
                dr["Approved Count"] = approvedCount.ToString("N0");
                dr["Rejected Count"] = rejectedCount.ToString("N0");
                dr["Pending Count"] = GlobalState.rules.PrintUsersPendingRules(user).Count();

                // Don't show or count rate if rate is Not an Answer (Divided by 0)
                if (!Double.IsNaN(successRate))
                {
                    dr["Success Rate"] = successRate.ToString("N0") + "%";
                    count++;
                    totalSuccess += Int32.Parse(successRate.ToString("N0"));
                } else dr["Success Rate"] = "";

                dt.Rows.Add(dr);
            }

            dr = dt.NewRow();
            dr["Average Success Rate"] = (totalSuccess/count).ToString("N2") + "%";
            dt.Rows.Add(dr);

            EditorStatisticsGridView.DataSource = dt;
            EditorStatisticsGridView.DataBind();
        }
    }
}