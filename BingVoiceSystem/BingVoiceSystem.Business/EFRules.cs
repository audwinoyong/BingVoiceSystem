using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BingVoiceSystem.Data;

namespace BingVoiceSystem
{
    public class EFRules
    {
        
        public EFRules()
        {

        }

        /*Takes in a question and returns the corresponding answer for that rule 
         * from the approvedrules table*/
        public string GetAnswer(string question)
        {
            //Remove extra whitespace and punctuation from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            //question = Regex.Replace(question, @"(\p{P}+)(?=\Z|\r\n)", "");
            using (var db = new BingDatabaseEntities())
            {
                var query = from r in db.PendingRules
                            where r.Question == question
                            select r.Answer;
                string result = query.FirstOrDefault();
                if (result != null)
                {
                    return result;
                }
            }
            return "Sorry, no result was found for that query";
        }

        public bool AddRule(string question, string response, string user, string table)
        {
            //Returns false if either question or response is empty
            if (question.Equals("") || response.Equals(""))
            {
                return false;
            }

            //Remove extra whitespace from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            using (var db = new BingDatabaseEntities())
            {
                var rule = new PendingRule
                {
                    Question = question,
                    Answer = response,
                    LastEditedBy = user
                };                db.PendingRules.Add(rule);
                db.SaveChanges();
                return true;
            }
        }

        public void DeleteRule(string question, string table)
        {
            using (var db = new BingDatabaseEntities())
            {
                var rule = (from r in db.PendingRules
                            where r.Question == question
                            select r).First();
                db.PendingRules.Remove(rule);
                db.SaveChanges();
            }
        }
    }
}
