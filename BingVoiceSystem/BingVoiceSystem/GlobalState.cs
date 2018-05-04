using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BingVoiceSystem
{
    public class GlobalState
    {
        /// <summary>
        /// A global state for the public objects, implemented from the database.
        /// </summary>
        public static readonly Rules rules = new Rules();
        public static readonly User user = new User();
        public static readonly MovieData movieData = new MovieData();
    }
}