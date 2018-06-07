using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BingVoiceSystem.Data;

namespace BingVoiceSystem
{
    /// <summary>
    /// Enum for rules tables.
    /// </summary>
    public enum Table { ApprovedRules, RejectedRules, PendingRules }

    /// <summary>
    /// Business logic for the Rules using Entity Framework.
    /// </summary>
    public class EFRules
    {
        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public EFRules()
        {
        }

        /// <summary>
        /// Takes in a question and returns the corresponding answer for that rule
        /// from the ApprovedRules table.
        /// </summary>
        /// <param name="question">The supplied question</param>
        /// <returns>The answer of the question</returns>
        public string GetAnswer(string question)
        {
            // Remove extra whitespace and punctuation from the question
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

        /// <summary>
        /// Takes in a question and returns the corresponding answer for that rule
        /// from the PendingRules table.
        /// </summary>
        /// <param name="question">The supplied question</param>
        /// <returns>The answer of the question</returns>
        public string GetAnswerFromPending(string question)
        {
            // Remove extra whitespace and punctuation from the question
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

        /// <summary>
        /// Return the wildcard of a given question if it meets any of data driven questions.
        /// </summary>
        /// <param name="askedQuestion">The supplied question</param>
        /// <returns>A list of best matches</returns>
        public List<string> GetWildCard(string askedQuestion)
        {
            // Get all data driven questions
            List<string> ApprovedQuestions = new List<string>();
            using (var db = new BingDBEntities())
            {
                ApprovedQuestions = (from r in db.ApprovedRules
                                     where r.Lookup != null
                                     select r.Question).ToList();
            }
            // Create list of potential matches
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
                    }
                    Matches.Add(new List<string> { SeparatedQuestion.Item1, askedQuestion.Substring(SeparatedQuestion.Item1.Length, askedQuestion.Length - SeparatedQuestion.Item1.Length - SeparatedQuestion.Item2.Length), SeparatedQuestion.Item2, Lookup, Answer });
                }
            }

            // Return best match
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

        /// <summary>
        /// Split a data driven question into its prefix (before wildcard) and suffix (after wildcard).
        /// </summary>
        /// <param name="question">The data driven question</param>
        /// <returns>A pair of prefix (before wildcard) and suffix (after wildcard)</returns>
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

        /// <summary>
        /// Remove the punctuations for a question.
        /// </summary>
        /// <param name="question">The question with punctuations</param>
        /// <returns>The question without punctuations</returns>
        public string RemovePuncForQuery(string question)
        {
            return question.Replace("?", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("<", "").
                Replace(">", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace(";", "");
        }

        /// <summary>
        /// Add a new rule into the specified Table.
        /// </summary>
        /// <param name="question">The question</param>
        /// <param name="response">The answer to the question</param>
        /// <param name="user">The current User</param>
        /// <param name="createdBy">The User who created the rule</param>
        /// <param name="lastEditedBy">The last User who edited the rule</param>
        /// <param name="Lookup">The lookup table for data driven rule</param>
        /// <param name="table">The specified Table</param>
        /// <returns>Error message if adding a rule fails</returns>
        public string AddRule(string question, string response, string user, string createdBy, string lastEditedBy, string Lookup, Table table)
        {
            // Returns false if either question or response is empty
            if (question == null || response == null)
            {
                return "Question and Answer fields are required.";
            }
            // If it is data driven, make sure it is in the right format.
            if (Lookup != null && !((response == "{Movies}" || response == "{Genres}" || response == "{Actors}") && (question.Contains("{%}"))))
            {
                return "You are attempting to make a data driven rule. Ensure your question contains {%} and your answer is either {Movies}, {Genres} or {Actors}.";
            }

            // Remove extra whitespace from the question
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

        /// <summary>
        /// Edit an existing rule.
        /// </summary>
        /// <param name="id">The ID of the rule</param>
        /// <param name="question">The new question</param>
        /// <param name="answer">The new answer</param>
        /// <param name="user">The current User</param>
        /// <param name="Lookup">The lookup table for data driven rule</param>
        /// <param name="table">The specified Table</param>
        /// <returns>Error message if editing a rule fails</returns>
        public string EditRule(int id, string question, string answer, string user, string Lookup, Table table)
        {
            // Returns false if either question or response is empty
            if (question == null || answer == null)
            {
                return "Question and Answer fields are required.";
            }

            // If it is data driven, make sure it is in the right format
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

        /// <summary>
        /// Delete a rule.
        /// </summary>
        /// <param name="question">The question</param>
        /// <param name="table">The specified Table</param>
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

        /// <summary>
        /// Check whether the question is already exists in the database
        /// </summary>
        /// <param name="question">The question supplied</param>
        /// <param name="RuleID"></param>
        /// <returns>True if the question already exists</returns>
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

        /// <summary>
        /// Approve a rule.
        /// </summary>
        /// <param name="question">The question</param>
        /// <param name="user">The current User</param>
        /// <param name="createdBy">The User who created the rule</param>
        /// <param name="lastEditedBy">The last User who edited the rule</param>
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

        /// <summary>
        /// Reject a rule.
        /// </summary>
        /// <param name="question">The question</param>
        /// <param name="user">The current User</param>
        /// <param name="createdBy">The User who created the rule</param>
        /// <param name="lastEditedBy">The last User who edited the rule</param>
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

        /// <summary>
        /// Print all pending rules.
        /// </summary>
        /// <returns>A list of all pending rules</returns>
        public List<PendingRule> PrintPendingRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.PendingRules
                            select r;
                return rules.ToList();
            }
        }

        /// <summary>
        /// Print all approved rules.
        /// </summary>
        /// <returns>A list of all approved rules</returns>
        public List<ApprovedRule> PrintApprovedRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.ApprovedRules
                            select r;
                return rules.ToList();
            }
        }

        /// <summary>
        /// Print all rejected rules.
        /// </summary>
        /// <returns>A list of all rejected rules</returns>
        public List<RejectedRule> PrintRejectedRules()
        {
            using (var db = new BingDBEntities())
            {
                var rules = from r in db.RejectedRules
                            select r;
                return rules.ToList();
            }
        }

        /// <summary>
        /// Print all pending rules of a specific User.
        /// </summary>
        /// <param name="user">The specified User</param>
        /// <returns>A list of all pending rules of the specified User</returns>
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

        /// <summary>
        /// Print all approved rules of a specific User.
        /// </summary>
        /// <param name="user">The specified User</param>
        /// <returns>A list of all approved rules of the specified User</returns>
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

        /// <summary>
        /// Print all rejected rules of a specific User.
        /// </summary>
        /// <param name="user">The specified User</param>
        /// <returns>A list of all rejected rules of the specified User</returns>
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

        /// <summary>
        /// Get the number of approved rules.
        /// </summary>
        /// <returns>The number of approved rules</returns>
        public int CountApproved()
        {
            return PrintApprovedRules().Count;
        }

        /// <summary>
        /// Get the number of rejected rules.
        /// </summary>
        /// <returns>The number of rejected rules</returns>
        public int CountRejected()
        {
            return PrintRejectedRules().Count;
        }

        /// <summary>
        /// Search for a pending rule based on its ID.
        /// </summary>
        /// <param name="id">The ID of a pending rule</param>
        /// <returns>The pending rule</returns>
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

        /// <summary>
        /// Search for an approved rule based on its ID.
        /// </summary>
        /// <param name="id">The ID of a approved rule</param>
        /// <returns>The approved rule</returns>
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

        /// <summary>
        /// Search for a rejected rule based on its ID.
        /// </summary>
        /// <param name="id">The ID of a rejected rule</param>
        /// <returns>The rejected rule</returns>
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

        /// <summary>
        /// Get the ID of a pending rule.
        /// </summary>
        /// <param name="question">The question</param>
        /// <returns>The ID of the pending rule</returns>
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

        /// <summary>
        /// Get the ID of an approved rule.
        /// </summary>
        /// <param name="question">The question</param>
        /// <returns>The ID of the approved rule</returns>
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
