using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BingVoiceSystem.Data;
using BingVoiceSystem.WebMVC.Controllers;
using BingVoiceSystem.WebMVC.Models;

namespace BingVoiceSystem
{
    [TestClass]
    public class RulesTest
    {
        private static EFRules efrules = new EFRules();
        private static Business.Data data = new Business.Data();

        [AssemblyInitialize]
        public static void SetupDataDirectory(TestContext context)
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            data.DataAdd("Test Movie", "Test Genre", new List<string> { "Actor1", "Actor2" }, "User");
            efrules.AddRule("What movies star {%}?", "{Movies}", "User", "User", "User", "Actors", Table.ApprovedRules);
            efrules.AddRule("What genre is {%}?", "{Genres}", "User", "User", "User", "Movies", Table.ApprovedRules);
            efrules.AddRule("What actors star in {%}?", "{Actors}", "User", "User", "User", "Movies", Table.ApprovedRules);
        }

        [TestMethod]
        public void DDRule_FindAnswerMovies_True()
        {
            Assert.IsTrue(efrules.GetAnswer("What movies star Actor1").Contains("Test Movie") && efrules.GetAnswer("What movies star Actor2").Contains("Test Movie"));
        }

        [TestMethod]
        public void DDRule_FindAnswerGenre_True()
        {
            Assert.IsTrue(efrules.GetAnswer("What Genre is test movie").Contains("Test Genre"));
        }

        [TestMethod]
        public void DDRule_FindAnswerActor_True()
        {
            Assert.IsTrue(efrules.GetAnswer("What actors star in test movie").Contains("Actor1, Actor2"));
        }

        [TestMethod]
        public void EditDDRule_EditSuccessful_True()
        {
            efrules.EditRule(efrules.GetApprovedID("What genre is {%}?"), "What is the genre of {%}?", "{Genres}", "User", "Movies", Table.ApprovedRules);
            Assert.IsTrue(efrules.GetAnswer("What is the genre of test movie").Contains("Test Genre"));
        }

        [TestMethod]
        public void EditDDData_EditSuccessful_True()
        {
            data.EditData(data.MovieNameToID("Test Movie"), "Not Test Movie", "Test Genre", new List<string> { "Actor1", "Actor2" }, "User");
            Assert.IsTrue(efrules.GetAnswer("What movies star Actor1").Contains("Not Test Movie"));
        }

        [TestMethod]
        public void EditDDRule_CannotFind_True()
        {
            efrules.EditRule(efrules.GetApprovedID("What actors star in {%}?"), "Who are the actors in {%}?", "{Actors}", "User", "Movies", Table.ApprovedRules);
            Assert.IsTrue(efrules.GetAnswer("What actors star in test movie").Contains("Sorry, no result was found for that query"));
        }

        [TestMethod]
        public void DeleteData_Successful_True()
        {
            data.DeleteData(data.MovieNameToID("Not Test Movie"));
            Assert.IsTrue(efrules.GetAnswer("What movies star Actor1").Contains("Sorry, no data was found for that query"));
            efrules.DeleteRule("What is the genre of {%}?", Table.ApprovedRules);
            efrules.DeleteRule("Who are the actors in {%}?", Table.ApprovedRules);
            efrules.DeleteRule("What movies star {%}?", Table.ApprovedRules);
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
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
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
            // adding duplicate question
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


        [TestMethod]
        public void HomeController_GetAnswer_AnswerFound()
        {
            // add and approve the rule
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.ApproveRule("Test Question?", "User", "User", "User");
            Assert.IsFalse(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));

            // create the home controller and mock rules model
            var controller = new HomeController();
            var model = new RulesModel { Question = "Test Question?" };

            // get the answer
            var result = controller.Index(model) as ViewResult;
            var response = (RulesModel)result.ViewData.Model;

            // answer is found
            Assert.AreEqual("Test Answer", response.Answer);

            // delete the rule
            efrules.DeleteRule("Test Question?", Table.ApprovedRules);
        }

        [TestMethod]
        public void HomeController_GetAnswer_AnswerNotFound()
        {
            // add and approve the rule
            efrules.AddRule("Test Question?", "Test Answer", "User", "User", "User", null, Table.PendingRules);
            efrules.ApproveRule("Test Question?", "User", "User", "User");
            Assert.IsFalse(efrules.GetAnswerFromPending("Test Question?").Contains("Test Answer"));

            // create the home controller and mock rules model
            var controller = new HomeController();
            var model = new RulesModel { Question = "Not a Test Question?" };

            // get the answer
            var result = controller.Index(model) as ViewResult;
            var response = (RulesModel)result.ViewData.Model;

            // answer is not found
            Assert.AreEqual("Sorry, no result was found for that query", response.Answer);

            // delete the rule
            efrules.DeleteRule("Test Question?", Table.ApprovedRules);
        }
    }
}
