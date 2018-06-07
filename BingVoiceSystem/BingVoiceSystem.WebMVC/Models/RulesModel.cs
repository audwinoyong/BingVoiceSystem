using System.ComponentModel.DataAnnotations;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    /// <summary>
    /// The data model for the Rules
    /// </summary>
    public class RulesModel
    {
        // The question of a rule. It should not be null and maximum length is 100 characters.
        [Required, MaxLength(100)]
        public string Question { get; set; }

        // The answer of a rule. It should not be null and maximum length is 100 characters.
        [Required, MaxLength(100)]
        public string Answer { get; set; }

        // The lookup table for the data-driven rule.
        public string Lookup { get; set; }

        // The approved rule.
        public ApprovedRule ApprovedRule { get; set; }

        // The rejected rule.
        public RejectedRule RejectedRule { get; set; }

        // The pending rule.
        public PendingRule PendingRule { get; set; }
    }
}