using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BingVoiceSystem.Data;

namespace BingVoiceSystem.WebMVC.Models
{
    public class RulesModel
    {        
        /// <summary>
        /// The name of a new personal contact
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The address for a new personal contact
        /// </summary>
        public string Address { get; set; }
    }
}