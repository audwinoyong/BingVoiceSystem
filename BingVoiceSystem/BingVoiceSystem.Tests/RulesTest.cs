using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BingVoiceSystem.Tests
{
    [TestClass]
    public class RulesTest
    {
        Rules rules = new Rules();

        [TestInitialize]
        public void Setup()
        {
            rules.AddRule("Am I a question?", "Yes, and I'm a response", "0", "PendingRules");
        }

        [TestMethod]
        public void GetAnswer_HappyResponse_True()
        {
            Assert.IsTrue(rules.GetAnswer("Am I a question?").Contains("Yes, and I'm a response"));
        }

        [TestMethod]
        public void GetAnswer_CaseDifferences_True()
        {
            Assert.IsTrue(rules.GetAnswer("Am I A QuEstiOn?").Contains("Yes, and I'm a response"));
        }

        [TestMethod]
        public void GetAnswer_BadlyFormattedQuestion_True()
        {
            Assert.IsTrue(rules.GetAnswer("Am  I A    QuEstiOn!!!?").Contains("Yes, and I'm a response"));
        }

        [TestMethod]
        public void GetAnswer_WrongResponse_False()
        {
            Assert.IsFalse(rules.GetAnswer("Am I a question?").Contains("Yes, and I am also a question"));
        }

        [TestMethod]
        public void GetAnswer_EmptyResponse_False()
        {
            Rules emptyRules = new Rules();

            Assert.IsFalse(emptyRules.GetAnswer("Am I a question?").Any());
        }

        /*Need to fix
        [TestMethod]
        public void EditRule_EditSuccessful_True()
        {
            rules.EditRule("Am I a question?", "Yes, and response has been edited", "0", "ApprovedRules");

            Assert.IsTrue(rules.GetAnswer("Am I a question?").Contains("Yes, and response has been edited"));
        }
        */

        /*Need to fix GetRulesList
        [TestMethod]
        public void DeleteRule_DeleteSuccessful_True()
        {
            rules.DeleteRule("Am I a question?", "ApprovedRules");

            Assert.IsTrue(rules.GetRulesList().Count == 0);
        }
        */

        [TestMethod]
        public void UpdateLastModifiedUser_UpdateSuccessful_True()
        {
            //TODO
        }
    }
}
