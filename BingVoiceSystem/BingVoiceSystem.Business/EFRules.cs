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
                var query = from r in db.ApprovedRules
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

        public string GetAnswerFromPending(string question)
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
                switch (table)
                {
                    case "ApprovedRules":
                        var apprule = new ApprovedRule
                        {
                            Question = question,
                            Answer = response,
                            ApprovedBy = user
                        };
                        db.ApprovedRules.Add(apprule);
                        break;
                    case "RejectedRules":
                        var rejrule = new RejectedRule
                        {
                            Question = question,
                            Answer = response,
                            RejectedBy = user
                        };
                        db.RejectedRules.Add(rejrule);
                        break;
                    case "PendingRules":
                        if (CheckExisting(question))
                        {
                            return false;
                        }
                        var penrule = new PendingRule
                        {
                            Question = question,
                            Answer = response,
                            LastEditedBy = user
                        };
                        db.PendingRules.Add(penrule);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown table");
                        return false;
                }
                db.SaveChanges();
                return true;
            }
        }

        public void DeleteRule(string question, string table)
        {
            using (var db = new BingDatabaseEntities())
            {
                switch (table)
                {
                    case "ApprovedRules":
                        var apprule = (from r in db.ApprovedRules
                                       where r.Question == question
                                       select r).First();
                        db.ApprovedRules.Remove(apprule);
                        break;
                    case "RejectedRules":
                        var rejrule = (from r in db.RejectedRules
                                       where r.Question == question
                                       select r).First();
                        db.RejectedRules.Remove(rejrule);
                        break;
                    case "PendingRules":
                        var penrule = (from r in db.PendingRules
                                       where r.Question == question
                                       select r).First();
                        db.PendingRules.Remove(penrule);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown table");
                        return;
                }
                db.SaveChanges();
            }
        }

        public bool CheckExisting(string question)
        {
            if (GetAnswer(question).Equals("Sorry, no result was found for that query") ||
                GetAnswerFromPending(question).Equals("Sorry, no result was found for that query"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApproveRule(string question, string user)
        {
            using (var db = new BingDatabaseEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.Question == question
                               select r).First();
                AddRule(penrule.Question, penrule.Answer, user, "ApprovedRules");
                db.PendingRules.Remove(penrule);
                db.SaveChanges();
            }
        }

        public void RejectRule(string question, string user)
        {
            using (var db = new BingDatabaseEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.Question == question
                               select r).First();
                AddRule(penrule.Question, penrule.Answer, user, "RejectedRules");
                db.PendingRules.Remove(penrule);
                db.SaveChanges();
            }
        }
    }
}
