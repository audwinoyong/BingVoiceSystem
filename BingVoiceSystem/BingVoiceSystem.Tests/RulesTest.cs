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
        private Rules rules = new Rules();

        [AssemblyInitialize]
        public static void SetupDataDirectory(TestContext context)
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void DatabaseOpens_Successful()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            conn.Close();
        }
        
        [TestMethod]
        public void GetAnswer_HappyResponse_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }
        
        
        [TestMethod]
        public void GetAnswer_CaseDifferences_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("TEsT quesTioN?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void GetAnswer_BadlyFormattedQuestion_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("    teSt      quEstiOn??").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void GetAnswer_WrongResponse_False()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("Not a Test Question?").Contains("Sorry, no answer was found for that query."));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void AddRule_EmptyQuestion_False()
        {
            Assert.IsFalse(rules.AddRule("", "Test Answer", "User", "PendingRules"));
            rules.DeleteRule("", "PendingRules");
        }

        [TestMethod]
        public void AddRule_EmptyAnswer_False()
        {
            Assert.IsFalse(rules.AddRule("Test Question?", "", "User", "PendingRules"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void AddRule_DuplicateQuestion_False()
        {
            Assert.IsTrue(rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules"));
            //adding duplicate question
            Assert.IsFalse(rules.AddRule("Test Question?", "Test Different Answer", "User", "PendingRules"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void DeleteRule_DeleteSuccessful_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.DeleteRule("Test Question?", "PendingRules");
            Assert.IsFalse(rules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
        }

        [TestMethod]
        public void EditQuestion_EditSuccessful_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "New Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("New Test Question?").Contains("Test Answer"));
            rules.DeleteRule("New Test Question?", "PendingRules");
        }

        [TestMethod]
        public void EditAnswer_EditSuccessful_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "Test Question?", "New Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("Test Question?").Contains("New Test Answer"));
            rules.DeleteRule("Test Question?", "PendingRules");
        }

        [TestMethod]
        public void EditQuestion_CannotFindRule_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.EditRule("Test Question?", "New Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(rules.GetAnswerFromPending("Test Question?").Contains("Sorry, no answer was found for that query."));
            rules.DeleteRule("New Test Question?", "PendingRules");
        }

        [TestMethod]
        public void ApproveRule_ExistsInApprovedTable_True()
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
        public void RejectRule_ExistsInRejectedTable_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.RejectRule("Test Question?", "User");
            string answer = "";
            DataTable d = rules.PrintRejectedRules();
            foreach (DataRow row in d.Rows)
            {
                if (row.ItemArray[0].Equals("Test Question?"))
                {
                    answer = (string)row.ItemArray[1];
                }
            }
            Assert.IsTrue(answer.Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "RejectedRules");
        }

        [TestMethod]
        public void ApproveRule_NoLongerInPendingTable_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.ApproveRule("Test Question?", "User");
            Assert.IsFalse(rules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "ApprovedRules");
        }

        [TestMethod]
        public void RejectRule_NoLongerInPendingTable_True()
        {
            rules.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            rules.RejectRule("Test Question?", "User");
            Assert.IsFalse(rules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            rules.DeleteRule("Test Question?", "RejectedRules");
        }

        [TestMethod]
        public void EFRule_FindAnswer_True()
        {
            EFRules ef = new EFRules();
            ef.AddRule("Test Question?", "Test Answer", "User", "PendingRules");
            Assert.IsTrue(ef.GetAnswer("Test Question?").Contains("Test Answer"));
            ef.DeleteRule("Test Question?", "PendingRules");
        }

    }
}
