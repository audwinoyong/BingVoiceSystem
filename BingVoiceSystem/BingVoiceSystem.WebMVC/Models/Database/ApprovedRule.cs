namespace BingVoiceSystem.WebMVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ApprovedRule
    {
        [Key]
        public int RuleID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        [StringLength(128)]
        public string ApprovedBy { get; set; }

        [StringLength(128)]
        public string LastEditedBy { get; set; }

        public bool DataDriven { get; set; }
    }
}
