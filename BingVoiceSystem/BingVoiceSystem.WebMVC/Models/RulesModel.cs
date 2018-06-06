using System.ComponentModel.DataAnnotations;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    public class RulesModel
    {        
        [Required, MaxLength(100)]
        public string Question { get; set; }

        [Required, MaxLength(100)]
        public string Answer { get; set; }

        public string Lookup { get; set; }

        public ApprovedRule ApprovedRule { get; set; }

        public RejectedRule RejectedRule { get; set; }

        public PendingRule PendingRule { get; set; }
    }
}