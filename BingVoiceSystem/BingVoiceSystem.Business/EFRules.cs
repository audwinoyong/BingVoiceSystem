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
            using (var db = new BingDBEntities())
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
            using (var db = new BingDBEntities())
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
            using (var db = new BingDBEntities())
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
                            LastEditedBy = user,
                            CreatedBy = user
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
            using (var db = new BingDBEntities())
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
            using (var db = new BingDBEntities())
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
            using (var db = new BingDBEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.Question == question
                               select r).First();
                AddRule(penrule.Question, penrule.Answer, user, "RejectedRules");
                db.PendingRules.Remove(penrule);
                db.SaveChanges();
            }
        }

        public void EditRule(int id, string question, string answer, string user, string table)
        {
            using (var db = new BingDBEntities())
            {
                switch (table)
                {
                    case "ApprovedRules":
                        var apprule = (from r in db.ApprovedRules
                                       where r.RuleID == id
                                       select r).First();
                        apprule.Question = question;
                        apprule.Answer = answer;
                        apprule.LastEditedBy = user;
                        break;
                    case "RejectedRules":
                        var rejrule = (from r in db.RejectedRules
                                       where r.RuleID == id
                                       select r).First();
                        rejrule.Question = question;
                        rejrule.Answer = answer;
                        rejrule.LastEditedBy = user;
                        break;
                    case "PendingRules":
                        var penrule = (from r in db.PendingRules
                                       where r.RuleID == id
                                       select r).First();
                        penrule.Question = question;
                        penrule.Answer = answer;
                        penrule.LastEditedBy = user;
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown table");
                        return;
                }
                db.SaveChanges();
            }
        }

        public List<PendingRule> PrintPendingRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.PendingRules
                            select r;
                return rules.ToList();
            }
        }

        public List<ApprovedRule> PrintApprovedRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.ApprovedRules
                            select r;
                return rules.ToList();
            }
        }

        public List<RejectedRule> PrintRejectedRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.RejectedRules
                            select r;
                return rules.ToList();
            }
        }

        public List<PendingRule> PrintUsersPendingRules(string user)
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.PendingRules
                            where r.LastEditedBy == user
                            select r;
                return rules.ToList();
            }
        }

        public List<ApprovedRule> PrintUsersApprovedRules(string user)
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.ApprovedRules
                            where r.ApprovedBy == user
                            select r;
                return rules.ToList();
            }
        }

        public List<RejectedRule> PrintUsersRejectedRules(string user)
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.RejectedRules
                            where r.RejectedBy == user
                            select r;
                return rules.ToList();
            }
        }

        /*Returns the number of approved rules.*/
        public int CountApproved()
        {
            return PrintApprovedRules().Count;
        }

        /*Returns the number of rejected rules.*/
        public int CountRejected()
        {
            return PrintRejectedRules().Count;
        }

        public PendingRule SearchPendingRule(int id)
        {
            using (var db = new BingDBEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.RuleID == id
                               select r).First();
                return penrule;
            }
        }

    }
}
