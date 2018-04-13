﻿using System;
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
            rules.AddRule("Am I a question?", "Yes, and I'm a response");
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


        [TestMethod]
        public void EditRule_EditSuccessful_True()
        {
            rules.EditRule("Am I a question?", "Yes, and response has been edited");

            Assert.IsTrue(rules.GetAnswer("Am I a question?").Contains("Yes, and response has been edited"));
        }

        [TestMethod]
        public void DeleteRule_DeleteSuccessful_True()
        {
            rules.DeleteRule("Am I a question?");

            Assert.IsTrue(rules.GetRulesList().Count == 0);
        }

        [TestMethod]
        public void UpdateLastModifiedUser_UpdateSuccessful_True()
        {
            //TODO
        }
    }
}
