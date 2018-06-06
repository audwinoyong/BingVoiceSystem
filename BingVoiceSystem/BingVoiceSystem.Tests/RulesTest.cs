using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections.Generic;
using BingVoiceSystem.Data;

namespace BingVoiceSystem
{
    [TestClass]
    public class RulesTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        // private Rules rules = new Rules();
        private EFRules efrules = new EFRules();

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
        public void EFRule_FindAnswer_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "CreatedBy", "LastEditedBy", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }
        
        
        [TestMethod]
        public void GetAnswer_CaseDifferences_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User","User", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("TEsT quesTioN?").Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void GetAnswer_BadlyFormattedQuestion_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("    teSt      quEstiOn??").Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void GetAnswer_WrongResponse_False()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("Not a Test Question?").Contains("Sorry, no result was found for that query"));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void AddRule_EmptyQuestion_False()
        {
            Assert.IsTrue(efrules.AddRule(null, "Test Answer", "User", "User", "User", null, Table.PendingRules).Contains("Question and Answer fields are required."));
        }

        [TestMethod]
        public void AddRule_EmptyAnswer_False()
        {
            Assert.IsTrue(efrules.AddRule("Test Question?", null, "User", "User", "User", null, Table.PendingRules).Contains("Question and Answer fields are required."));
        }

        [TestMethod]
        public void AddRule_DuplicateQuestion_False()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            //adding duplicate question
            Assert.IsTrue(efrules.AddRule("Test Question?", "Test Different Answer", "User", "User", "User", null, Table.PendingRules).Contains("This question already exists, please use another."));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void DeleteRule_DeleteSuccessful_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.DeleteRule("Test Question?", Table.PendingRules);
            Assert.IsFalse(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
        }

        [TestMethod]
        public void EditQuestion_EditSuccessful_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.EditRule(efrules.GetPendingID("Test Question?"), "New Test Question?", "Test Answer", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("New Test Question?").Contains("Test Answer"));
            efrules.DeleteRule("New Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void EditAnswer_EditSuccessful_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.EditRule(efrules.GetPendingID("Test Question?"), "Test Question?", "New Test Answer", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("Test Question?").Contains("New Test Answer"));
            efrules.DeleteRule("Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void EditQuestion_CannotFindRule_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.EditRule(efrules.GetPendingID("Test Question?"), "New Test Question?", "Test Answer", "User", null, Table.PendingRules);
            Assert.IsTrue(efrules.GetAnswerFromPending("Test Question?").Contains("Sorry, no result was found for that query"));
            efrules.DeleteRule("New Test Question?", Table.PendingRules);
        }

        [TestMethod]
        public void ApproveRule_ExistsInApprovedTable_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.ApproveRule("Test Question?", "User", "User", "User");
            string answer = "";
            List<ApprovedRule> d = efrules.PrintApprovedRules();
            foreach (ApprovedRule row in d)
            {
                if (row.Question.Equals("Test Question?"))
                {
                    answer = row.Answer;
                }
            }
            Assert.IsTrue(answer.Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.ApprovedRules);
        }

        [TestMethod]
        public void RejectRule_ExistsInRejectedTable_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.RejectRule("Test Question?", "User", "User", "User");
            string answer = "";
            List<RejectedRule> d = efrules.PrintRejectedRules();
            foreach (RejectedRule row in d)
            {
                if (row.Question.Equals("Test Question?"))
                {
                    answer = row.Answer;
                }
            }
            Assert.IsTrue(answer.Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.RejectedRules);
        }

        [TestMethod]
        public void ApproveRule_NoLongerInPendingTable_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.ApproveRule("Test Question?", "User", "User", "User");
            Assert.IsFalse(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.ApprovedRules);
        }

        [TestMethod]
        public void RejectRule_NoLongerInPendingTable_True()
        {
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.RejectRule("Test Question?", "User", "User", "User");
            Assert.IsFalse(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));
            efrules.DeleteRule("Test Question?", Table.RejectedRules);
        }



    }
}
