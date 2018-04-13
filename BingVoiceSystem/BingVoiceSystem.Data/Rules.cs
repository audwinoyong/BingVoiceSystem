using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;


namespace BingVoiceSystem
{

    public class Rules
    {

        private Dictionary<string, string> rulesList;
        private Dictionary<string, string> rejectedRulesList;

        public Rules()
        {
            rulesList = new Dictionary<string, string>();
            rejectedRulesList = new Dictionary<string, string>();
        }

        public void AddRule(string question, string response)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, "[^\\w\\s]", "");
            rulesList.Add(question, response);
        }

        public void EditRule(string question, string response)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, "[^\\w\\s]", "");

            if (rulesList.ContainsKey(question))
            {
                rulesList[question] = response;
            }
        }

        public void DeleteRule(string key)
        {
            key = Regex.Replace(key, "\\s+", " ").Trim();
            key = Regex.Replace(key, "[^\\w\\s]", "");

            try
            {
                rulesList.Remove(key);
            }
            catch (KeyNotFoundException) { }
        }

        public void RejectRule(string question, string response)
        {
            rejectedRulesList.Add(question, response);
            DeleteRule(question);
        }

        public Dictionary<string, string> GetRulesList()
        {
            return rulesList;
        }

        public Dictionary<string, string> GetRejectedRulesList()
        {
            return rejectedRulesList;
        }

        public void PrintRules()
        {
            foreach (var pair in rulesList)
            {
                Console.WriteLine(pair.Key + ", " + pair.Value);
            }
        }

        public IEnumerable<string> GetAnswer(string key)
        {
            key = Regex.Replace(key, "\\s+", " ").Trim();
            key = Regex.Replace(key, "[^\\w\\s]", "");
            return from r in rulesList
                   where r.Key.ToLower().Contains(key.ToLower())
                   select r.Value;
        }

        public KeyValuePair<string, string> GetRejectedRule(string key)
        {
            KeyValuePair<string, string> result = new KeyValuePair<string, string>();
            foreach (var pair in rejectedRulesList)
            {
                if (pair.Key.Equals(key))
                {
                    return pair;
                }
            }
            return result;
        }
    }
}
