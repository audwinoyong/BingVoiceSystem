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
            question = removePuncForQuery(question);

            if (GetWildCard(question).Count > 0)
            {
                Business.Data data = new Business.Data();
                List<string> DataList = GetWildCard(question);
                return data.GetData(DataList[1], DataList[0], DataList[2]);
            }
            else
            {
                using (var db = new BingDBEntities())
                {
                    var query = from r in db.ApprovedRules
                                where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.ToLower()
                                select r.Answer;
                    string result = query.FirstOrDefault();
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return "Sorry, no result was found for that query";
        }

        //Returns the wildcard of a given question if it meets any of data driven questions.
        public List<string> GetWildCard(string askedQuestion)
        {
            //Get all data driven questions
            List<string> ApprovedQuestions = new List<string>();
            using (var db = new BingDBEntities())
            {
                ApprovedQuestions = db.ApprovedRules.Where(q => q.Lookup != null).Select(q => q.Question).ToList();

            }
            //Create list of potential matches
            List<List<string>> Matches = new List<List<string>>();
            foreach (string question in ApprovedQuestions)
            {
                string comparison = question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower();

                Tuple<string, string> SeparatedQuestion = GetQuestionPrefixSufix(comparison);

                if (askedQuestion.IndexOf(SeparatedQuestion.Item1, StringComparison.OrdinalIgnoreCase) >= 0 && askedQuestion.IndexOf(SeparatedQuestion.Item2, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string Answer;
                    string Lookup;
                    using (var db = new BingDBEntities())
                    {
                        Answer = db.ApprovedRules.Where(q => q.Question.Equals(question)).Select(q => q.Answer).First();
                        Lookup = db.ApprovedRules.Where(q => q.Question.Equals(question)).Select(q => q.Lookup).First();
                    }
                    Matches.Add(new List<string> { SeparatedQuestion.Item1, askedQuestion.Substring(SeparatedQuestion.Item1.Length, askedQuestion.Length - SeparatedQuestion.Item1.Length - SeparatedQuestion.Item2.Length), SeparatedQuestion.Item2, Lookup, Answer });
                }
            }

            //Return best match
            int LargestPrefix = 0;
            int LargestSufix = 0;
            List<string> BestMatch = new List<string>();

            foreach (List<string> match in Matches)
            {
                if (match[0].Length >= LargestPrefix && match[2].Length >= LargestSufix)
                {
                    BestMatch.Clear();
                    BestMatch.Add(match[1]);
                    BestMatch.Add(match[3]);
                    BestMatch.Add(match[4]);

                    LargestPrefix = match[0].Length;
                    LargestSufix = match[2].Length;
                }
            }

            return BestMatch;
        }

        //Splits a data driven question into its prefix (before wildcard) and sufix (after wildcard)
        public Tuple<string, string> GetQuestionPrefixSufix(string question)
        {
            string Prefix = "";
            string Sufix = "";
            bool MetWildcard = false;

            foreach (char c in question)
            {
                if (c == '{' || c == '%' || c == '}')
                {
                    MetWildcard = true;
                }
                else if (!MetWildcard)
                {
                    Prefix += c;
                }
                else Sufix += c;
            }

            return Tuple.Create(Prefix, Sufix);
        }

        public string removePuncForQuery(string question)
        {
            return question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "");
        }

        public bool AddRule(string question, string response, string user, string createdBy, string lastEditedBy, string Lookup, string table)
        {
            //Returns false if either question or response is empty
            if (question.Equals("") || response.Equals(""))
            {
                return false;
            }

            //If it is data driven, make sure it is in the right format.
            if (Lookup != null && !((response == "{Movie}" || response == "{Genre}" || response == "{Actors}") && (question.Contains("{%}"))))
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
                            ApprovedBy = user,
                            LastEditedBy = lastEditedBy,
                            CreatedBy = createdBy,
                            Lookup = Lookup
                        };
                        db.ApprovedRules.Add(apprule);
                        break;
                    case "RejectedRules":
                        var rejrule = new RejectedRule
                        {
                            Question = question,
                            Answer = response,
                            RejectedBy = user,
                            LastEditedBy = lastEditedBy,
                            CreatedBy = createdBy,
                            Lookup = Lookup
                        };
                        db.RejectedRules.Add(rejrule);
                        break;
                    case "PendingRules":
                        if (!CheckExisting(question))
                        {
                            return false;
                        }
                        var penrule = new PendingRule
                        {
                            Question = question,
                            Answer = response,
                            LastEditedBy = user,
                            CreatedBy = user,
                            Lookup = Lookup
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
            string ApprovedCheck;
            string RejectedCheck;
            string PendingCheck;

            using (var db = new BingDBEntities())
            {
                var query = from r in db.ApprovedRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.ToLower()
                            select r.Answer;
                ApprovedCheck = query.FirstOrDefault();

                query = from r in db.ApprovedRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.ToLower()
                            select r.Answer;
                RejectedCheck = query.FirstOrDefault();
                query = from r in db.ApprovedRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.ToLower()
                            select r.Answer;
                PendingCheck = query.FirstOrDefault();
            }

            if (ApprovedCheck != null || RejectedCheck != null || PendingCheck != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApproveRule(string question, string user, string createdBy, string lastEditedBy)
        {
            using (var db = new BingDBEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.Question == question
                               select r).First();
                AddRule(penrule.Question, penrule.Answer, user, createdBy, lastEditedBy, penrule.Lookup, "ApprovedRules");
                db.PendingRules.Remove(penrule);
                db.SaveChanges();
            }
        }

        public void RejectRule(string question, string user, string createdBy, string lastEditedBy)
        {
            using (var db = new BingDBEntities())
            {
                var penrule = (from r in db.PendingRules
                               where r.Question == question
                               select r).First();
                AddRule(penrule.Question, penrule.Answer, user, createdBy, lastEditedBy, penrule.Lookup, "RejectedRules");
                db.PendingRules.Remove(penrule);
                db.SaveChanges();
            }
        }

        public bool EditRule(int id, string question, string answer, string user, string Lookup, string table)
        {
            //Returns false if either question or response is empty
            if (question == null || answer == null)
            {
                return false;
            }

            //If it is data driven, make sure it is in the right format.
            if (Lookup != null && !((answer == "{Movie}" || answer == "{Genre}" || answer == "{Actors}") && (question.Contains("{%}"))))
            {
                return false;
            }

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
                        apprule.Lookup = Lookup;
                        break;
                    case "RejectedRules":
                        var rejrule = (from r in db.RejectedRules
                                       where r.RuleID == id
                                       select r).First();
                        rejrule.Question = question;
                        rejrule.Answer = answer;
                        rejrule.LastEditedBy = user;
                        rejrule.Lookup = Lookup;
                        break;
                    case "PendingRules":
                        var penrule = (from r in db.PendingRules
                                       where r.RuleID == id
                                       select r).First();
                        penrule.Question = question;
                        penrule.Answer = answer;
                        penrule.LastEditedBy = user;
                        penrule.Lookup = Lookup;
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Unknown table");
                        return false;
                }
                db.SaveChanges();
                return true;
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

        public ApprovedRule SearchApprovedRule(int id)
        {
            using (var db = new BingDBEntities())
            {
                var apprule = (from r in db.ApprovedRules
                               where r.RuleID == id
                               select r).First();
                return apprule;
            }
        }

        public RejectedRule SearchRejectedRule(int id)
        {
            using (var db = new BingDBEntities())
            {
                var rejrule = (from r in db.RejectedRules
                               where r.RuleID == id
                               select r).First();
                return rejrule;
            }
        }
    }
}
