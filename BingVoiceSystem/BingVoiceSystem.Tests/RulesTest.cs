using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace BingVoiceSystem
{
    [TestClass]
    public class RulesTest
    {
        // private Rules rules = new Rules();


        [TestMethod]
        public void DatabaseOpens()
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            conn.Close();
        }
        
        [TestMethod]
        public void GetAnswer_HappyResponse_True()
        {
            Rules rules = new Rules();
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("Test Question?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }
        
        /*
        [TestMethod]
        public void GetAnswer_CaseDifferences_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("TEsT queTtioN?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void GetAnswer_BadlyFormattedQuestion_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("  teSt    qEestiOn??!?!?!?!").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void GetAnswer_WrongResponse_False()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("Not a Test Question?").Contains("Sorry, no answer was found for that query."));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void DeleteRule_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.DeleteRule("Test Question?", "PendingRules");
            Assert.IsFalse(rules.GetAnswer("Test Question?").Contains("Test Answer"));
        }

        [TestMethod]
        public void EditQuestion_EditSuccessful_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "New Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("New Test Question?").Contains("Test Answer"));
            rules.DeleteRule("New Test Question?", "PendingRules");
        }

        public void EditAnswer_EditSuccessful_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "Test Question?", "New Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("Test Question?").Contains("New Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        public void EditQuestion_CantFindRule_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "New Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswer("Test Question?").Contains("Sorry, no answer was found for that query."));
            rules.DeleteRule("New Test Question?", "PendingRules");
        }

        [TestMethod]
        public void ApproveRule_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.ApproveRule("Test Question?", "User");
            string answer = "";
            DataTable d = rules.PrintApprovedRules();
            foreach (DataRow row in d.Rows)
            {
                if (row.ItemArray[0].Equals("Test Question?"))
                {
                    answer = (string)row.ItemArray[1];
                }
            }
            Assert.IsTrue(answer.Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "ApprovedRules");
        }

        [TestMethod]
        public void UpdateLastModifiedUser_UpdateSuccessful_True()
        {
            //TODO
        }*/
    }
}
