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
        protected void Page_Load(object sender, EventArgs e)
        {
            Rules rules = new Rules();
            Dictionary<string, string> ruleslist = rules.PrintApprovedRules();
            foreach (KeyValuePair<string, string> pair in ruleslist)
            {
                ListBox1.Items.Add(new ListItem(pair.Value, pair.Key));
            }
        }
    }
}