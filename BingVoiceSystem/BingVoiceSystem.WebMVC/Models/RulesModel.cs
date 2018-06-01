﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    public class RulesModel
    {        
        public EFRules EFRules { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public RulesModel()
        {
            EFRules = new EFRules();
            Answer = "";
        }

        public void SetAnswer(string question)
        {
            Answer = EFRules.GetAnswer(question);
        }

        public List<PendingRule> PendingRules { get; set; }

        public List<ApprovedRule> ApprovedRules { get; set; }

        public List<RejectedRule> RejectedRules { get; set; }

        public void AddRule(string question, string response, string name)
        {
            EFRules.AddRule(question, response, name, "PendingRules");
        }
    }
}