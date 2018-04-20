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

        public void AddRule(string question, string response, string ID, string table)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, "[^\\w\\s]", "");
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
                        query = @"INSERT INTO PendingRules (Question, Answer) Values(@q, @a)";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                cmd.Parameters.Add(new SqlParameter("a", response));
                cmd.Parameters.Add(new SqlParameter("i", ID));

                cmd.ExecuteNonQuery();
            }
        }

        public void EditRule(string question, string answer, string table)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = "";
                switch (table)
                {
                    case "ApprovedRules":
                        query = @"Update ApprovedRules Set Answer = @a Where Question = @q";
                        break;
                    case "RejectedRules":
                        query = @"Update RejectedRules Set Answer = @a Where Question = @q";
                        break;
                    case "PendingRules":
                        query = @"Update PendingRules Set Answer = @a Where Question = @q";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                cmd.Parameters.Add(new SqlParameter("a", answer));
                
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
                        query = @"Delete From ApprovedRules Where Question = @q";
                        break;
                    case "RejectedRules":
                        query = @"Delete From RejectedRules Where Question = @q";
                        break;
                    case "PendingRules":
                        query = @"Delete From PendingRules Where Question = @q";
                        break;
                    default:
                        break;
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                
                cmd.ExecuteNonQuery();
            }
        }

        public void RejectRule(string question, string ID)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                string rejectedquestion;
                string rejectedanswer;
                conn.Open();
                string query = @"select Question, Answer from PendingRules Where Question = @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        rejectedquestion = rdr.GetString(0);
                        rejectedanswer = rdr.GetString(1);
                        DeleteRule(question, "PendingRules");
                        AddRule(rejectedquestion, rejectedanswer, ID, "RejectedRules");
                    }
                }
            }
        }

        public void ApproveRule(string question, string ID)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                string approvedquestion;
                string approvedanswer;
                conn.Open();
                string query = @"select Question, Answer from PendingRules Where Question = @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        approvedquestion = rdr.GetString(0);
                        approvedanswer = rdr.GetString(1);
                        DeleteRule(question, "PendingRules");
                        AddRule(approvedquestion, approvedanswer, ID, "ApprovedRules");
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
                SqlCommand cmd = new SqlCommand("select Question, Answer from ApprovedRules", conn);
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
                SqlCommand cmd = new SqlCommand("select Question, Answer from RejectedRules", conn);
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
                SqlCommand cmd = new SqlCommand("select Question, Answer from PendingRules", conn);
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

        /*TODO
        public Dictionary<string, string> GetRulesList()
        {
            Dictionary<string,string> rules = new Dictionary<string,string>();
            using (SqlConnection conn = new SqlConnection(path))

        }
        */

        public string GetAnswer(string question)
        {
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, "[^\\w\\s]", "");
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                string query = @"Select Answer From ApprovedRules Where Question = @q";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("q", question));
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

        public Dictionary<string, string> PrintUserRules(string ID)
        {
            if (ID == null)
            {
                ID = "0";
            }

            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Question, Answer, ApprovedBy FROM ApprovedRules UNION SELECT Question, Answer, RejectedBy FROM RejectedRules", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (rdr.GetString(2) == ID)
                        {
                            ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                        }
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
