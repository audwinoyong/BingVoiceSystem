using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    public class RulesModel
    {        
        public EFRules EFRules { get; set; }

        public int RuleID { get; set; }

        [Required, MaxLength(100)]
        public string Question { get; set; }

        [Required, MaxLength(100)]
        public string Answer { get; set; }

        public ApprovedRule ApprovedRule { get; set; }

        public RejectedRule RejectedRule { get; set; }

        public PendingRule PendingRule { get; set; }

        public RulesModel()
        {
            EFRules = new EFRules();
            Answer = "";

            ApprovedRule = new ApprovedRule();
            RejectedRule = new RejectedRule();
            PendingRule = new PendingRule();
        }

        public void SetAnswer(string question)
        {
            Answer = EFRules.GetAnswer(question);
        }

        //public void AddRule(string question, string response, string name)
        //{
        //    EFRules.AddRule(question, response, name, "PendingRules");
        //}
    }
}