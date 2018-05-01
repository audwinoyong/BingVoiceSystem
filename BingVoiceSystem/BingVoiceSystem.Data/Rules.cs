using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;


namespace BingVoiceSystem
{

    public class Rules
    {

        string path;

        public Rules()
        {
            path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool CheckExisting(string question)
        {
            question = Regex.Replace(question, "[^\\w\\s]", "");
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = @"SELECT Question FROM PendingRules";
                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string response = Regex.Replace(rdr.GetString(0), "[^\\w\\s]", "");
                        if (question.ToLower().Equals(response.ToLower()))
                        {
                            return false;
                        }
                    }
                }
                query = @"SELECT Question FROM ApprovedRules";
                cmd = new SqlCommand(query, conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string response = Regex.Replace(rdr.GetString(0), "[^\\w\\s]", "");
                        if (question.ToLower().Equals(response.ToLower()))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool AddRule(string question, string response, string user, string table)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = "";
                switch (table)
                {
                    case "ApprovedRules":
                        query = @"INSERT INTO ApprovedRules (Question, Answer, ApprovedBy) Values(@q, @a, @i)";
                        break;
                    case "RejectedRules":
                        query = @"INSERT INTO RejectedRules (Question, Answer, RejectedBy) Values(@q, @a, @i)";
                        break;
                    case "PendingRules":
                        if (!CheckExisting(question))
                        {
                            return false;
                        }
                        query = @"INSERT INTO PendingRules (Question, Answer, LastEditedBy) Values(@q, @a, @i)";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                cmd.Parameters.Add(new SqlParameter("a", response));
                cmd.Parameters.Add(new SqlParameter("i", user));

                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public void EditRule(string oldquestion, string question, string answer, string user, string table)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = "";
                switch (table)
                {
                    case "ApprovedRules":
                        query = @"UPDATE ApprovedRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @oq";
                        break;
                    case "RejectedRules":
                        query = @"UPDATE RejectedRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @oq";
                        break;
                    case "PendingRules":
                        query = @"UPDATE PendingRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @o";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                cmd.Parameters.Add(new SqlParameter("a", answer));
                cmd.Parameters.Add(new SqlParameter("i", user));
                cmd.Parameters.Add(new SqlParameter("o", oldquestion));
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteRule(string question, string table)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = "";
                switch (table)
                {
                    case "ApprovedRules":
                        query = @"DELETE FROM ApprovedRules WHERE Question = @q";
                        break;
                    case "RejectedRules":
                        query = @"DELETE FROM RejectedRules WHERE Question = @q";
                        break;
                    case "PendingRules":
                        query = @"DELETE FROM PendingRules WHERE Question = @q";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                
                cmd.ExecuteNonQuery();
            }
        }

        public void RejectRule(string question, string user)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                string rejectedQuestion;
                string rejectedAnswer;
                conn.Open();
                string query = @"SELECT Question, Answer FROM PendingRules WHERE Question = @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        rejectedQuestion = rdr.GetString(0);
                        rejectedAnswer = rdr.GetString(1);
                        DeleteRule(question, "PendingRules");
                        AddRule(rejectedQuestion, rejectedAnswer, user, "RejectedRules");
                    }
                }
            }
        }

        public void ApproveRule(string question, string user)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                string approvedQuestion;
                string approvedAnswer;
                conn.Open();
                string query = @"SELECT Question, Answer FROM PendingRules WHERE Question = @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        approvedQuestion = rdr.GetString(0);
                        approvedAnswer = rdr.GetString(1);
                        DeleteRule(question, "PendingRules");
                        AddRule(approvedQuestion, approvedAnswer, user, "ApprovedRules");
                    }
                }
            }
        }

        public Dictionary<string, string> PrintApprovedRules()
        {
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer FROM ApprovedRules", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }
            return ruleslist;
        }

        public Dictionary<string, string> PrintRejectedRules()
        {
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer FROM RejectedRules", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }
            return ruleslist;
        }

        public Dictionary<string, string> PrintPendingRules()
        {
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer FROM PendingRules", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }
            return ruleslist;
        }

        public string GetAnswer(string question)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, "[^\\w\\s]", "");
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = @"SELECT Answer FROM ApprovedRules WHERE LOWER(Question) LIKE @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question.ToLower() + "%"));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return rdr.GetString(0);
                    }
                    else
                    {
                        return "Sorry, no answer was found for that query.";
                    }
                }
            }
        }

        // Returns all approved rules of a specific user
        public Dictionary<string, string> PrintUsersApprovedRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer, ApprovedBy FROM ApprovedRules WHERE ApprovedBy = @i", conn);
                cmd.Parameters.Add(new SqlParameter("i", user.ConvertIdToUsername(userId)));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }

            return ruleslist;
        }

        // Returns all rejected rules of a specific user
        public Dictionary<string, string> PrintUsersRejectedRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer, RejectedBy FROM RejectedRules WHERE RejectedBy = @u", conn);
                cmd.Parameters.Add(new SqlParameter("u", user.ConvertIdToUsername(userId)));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }
            return ruleslist;
        }

        // Returns all pending rules of a specific user
        public Dictionary<string, string> PrintUsersPendingRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer, LastEditedBy FROM PendingRules WHERE LastEditedBy = @i", conn);
                cmd.Parameters.Add(new SqlParameter("i", user.ConvertIdToUsername(userId)));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                    }
                }
            }
            return ruleslist;
        }

        public int CountApproved()
        {
            return PrintApprovedRules().Count();
        }

        public int CountRejected()
        {
            return PrintRejectedRules().Count();
        }
    }
}
