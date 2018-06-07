using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BingVoiceSystem.Data;

namespace BingVoiceSystem
{
    // Enum for rules tables
    public enum Table { ApprovedRules, RejectedRules, PendingRules }

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
            question = RemovePuncForQuery(question);
            List<string> wildCard;
            if ((wildCard = GetWildCard(question)).Count > 0)
            {
                Business.Data data = new Business.Data();
                return data.GetData(wildCard[1], wildCard[0], wildCard[2]);
                
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

        public string GetAnswerFromPending(string question)
        {
            //Remove extra whitespace and punctuation from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = RemovePuncForQuery(question);
            List<string> wildCard;
            if ((wildCard = GetWildCard(question)).Count > 0)
            {
                Business.Data data = new Business.Data();
                return data.GetData(wildCard[1], wildCard[0], wildCard[2]);
            }
            else
            {
                using (var db = new BingDBEntities())
                {
                    var query = from r in db.PendingRules
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
                ApprovedQuestions = (from r in db.ApprovedRules
                                    where r.Lookup != null
                                    select r.Question).ToList();
                 //   db.ApprovedRules.Where(q => q.Lookup != null).Select(q => q.Question).ToList();

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
                        Answer = (from r in db.ApprovedRules
                                 where r.Question.Equals(question)
                                 select r.Answer).First();
                        Lookup = (from r in db.ApprovedRules
                                  where r.Question.Equals(question)
                                  select r.Lookup).First();
                        //Answer = db.ApprovedRules.Where(q => q.Question.Equals(question)).Select(q => q.Answer).First();
                        //Lookup = db.ApprovedRules.Where(q => q.Question.Equals(question)).Select(q => q.Lookup).First();
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

        public string RemovePuncForQuery(string question)
        {
            return question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "");
        }

        public string AddRule(string question, string response, string user, string createdBy, string lastEditedBy, string Lookup, Table table)
        {
            //Returns false if either question or response is empty
            if (question == null || response == null)
            {
                return "Question and Answer fields are required.";
            }
            //If it is data driven, make sure it is in the right format.
            if (Lookup != null && !((response == "{Movies}" || response == "{Genres}" || response == "{Actors}") && (question.Contains("{%}"))))
            {
                return "You are attempting to make a data driven rule. Ensure your question contains {%} and your answer is either {Movies}, {Genres} or {Actors}.";
            }

            //Remove extra whitespace from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            using (var db = new BingDBEntities())
            {
                question = question.TrimEnd(' ');

                switch (table)
                {
                    case Table.ApprovedRules:
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
                    case Table.RejectedRules:
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
                    case Table.PendingRules:
                        if (CheckExisting(question, -1))
                        {
                            return "This question already exists, please use another.";
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
                        return "Sorry something went wrong. Try reload the page and try again.";
                }
                db.SaveChanges();
                return null;
            }
        }

        public string EditRule(int id, string question, string answer, string user, string Lookup, Table table)
        {
            //Returns false if either question or response is empty
            if (question == null || answer == null)
            {
                return "Question and Answer fields are required.";
            }

            //If it is data driven, make sure it is in the right format.
            if (Lookup != null && !((answer == "{Movies}" || answer == "{Genres}" || answer == "{Actors}") && (question.Contains("{%}"))))
            {
                return "You are attempting to make a data driven rule. Ensure your question contains {%} and your answer is either {Movies}, {Genres} or {Actors}.";
            }

            using (var db = new BingDBEntities())
            {
                question = question.TrimEnd(' ');

                if (CheckExisting(question, id))
                {
                    return "This question already exists, please use another.";
                }

                switch (table)
                {
                    case Table.ApprovedRules:
                        var apprule = (from r in db.ApprovedRules
                                       where r.RuleID == id
                                       select r).First();
                        apprule.Question = question;
                        apprule.Answer = answer;
                        apprule.LastEditedBy = user;
                        apprule.Lookup = Lookup;
                        break;
                    case Table.RejectedRules:
                        var rejrule = (from r in db.RejectedRules
                                       where r.RuleID == id
                                       select r).First();
                        rejrule.Question = question;
                        rejrule.Answer = answer;
                        rejrule.LastEditedBy = user;
                        rejrule.Lookup = Lookup;
                        break;
                    case Table.PendingRules:
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
                        return "Sorry something went wrong. Try reload the page and try again.";
                }
                db.SaveChanges();
                return null;
            }
        }

        public void DeleteRule(string question, Table table)
        {
            using (var db = new BingDBEntities())
            {
                switch (table)
                {
                    case Table.ApprovedRules:
                        var apprule = (from r in db.ApprovedRules
                                       where r.Question == question
                                       select r).First();
                        db.ApprovedRules.Remove(apprule);
                        break;
                    case Table.RejectedRules:
                        var rejrule = (from r in db.RejectedRules
                                       where r.Question == question
                                       select r).First();
                        db.RejectedRules.Remove(rejrule);
                        break;
                    case Table.PendingRules:
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

        public bool CheckExisting(string question, int RuleID)
        {
            int ApprovedCheck;
            int RejectedCheck;
            int PendingCheck;

            using (var db = new BingDBEntities())
            {
                var query = from r in db.ApprovedRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                            select r.RuleID;
                ApprovedCheck = query.FirstOrDefault();

                query = from r in db.RejectedRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                            select r.RuleID;
                RejectedCheck = query.FirstOrDefault();

                query = from r in db.PendingRules
                            where r.Question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                                    == question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                                    Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "").ToLower()
                            select r.RuleID;
                PendingCheck = query.FirstOrDefault();
            }

            if ((ApprovedCheck != 0 || RejectedCheck != 0 || PendingCheck != 0) && (ApprovedCheck != RuleID && RejectedCheck != RuleID && PendingCheck != RuleID))
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
                AddRule(penrule.Question, penrule.Answer, user, createdBy, lastEditedBy, penrule.Lookup, Table.ApprovedRules);
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
                AddRule(penrule.Question, penrule.Answer, user, createdBy, lastEditedBy, penrule.Lookup, Table.RejectedRules);
                db.PendingRules.Remove(penrule);
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

        public int GetPendingID(string question)
        {
            using (var db = new BingDBEntities())
            {
                var id = (from r in db.PendingRules
                               where r.Question == question
                               select r.RuleID).First();
                return id;
            }
        }

        public int GetApprovedID(string question)
        {
            using (var db = new BingDBEntities())
            {
                var id = (from r in db.ApprovedRules
                          where r.Question == question
                          select r.RuleID).First();
                return id;
            }
        }
    }
}
