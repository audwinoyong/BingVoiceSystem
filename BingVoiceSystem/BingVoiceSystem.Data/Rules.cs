using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Data;

namespace BingVoiceSystem
{

    public class Rules
    {
        //Path to database
        public string path;

        public Rules(string path)
        {
            this.path = path;
        }

        /*Constructor: creates and sets the path to the database*/
        public Rules()
        {
            path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }

        /*Takes in a question as a string and checks if it already exists in the pending or
         * approved database tables. Returns true if it exists*/
        public bool CheckExisting(string question)
        {
            //Remove punctuation
            question = Regex.Replace(question, "[^\\w\\s]", "");
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = @"SELECT Question FROM PendingRules";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    //Check if the question exists in the pendingrules table
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
                    //check if question exists in the approvedrules table
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
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return true;
        }

        /*Adds a new rule to the appropriate table. Takes in a question and a response for the rule,
         * a user for who created the rule and the table that the rule is to be added to*/
        public bool AddRule(string question, string response, string user, string table, bool isDataDriven)
        {
            //Remove extra whitespace from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = "";
                    //Change query depending on the table
                    //If pendingrules table check if the rule already exists
                    switch (table)
                    {
                        case "ApprovedRules":
                            query = @"INSERT INTO ApprovedRules (Question, Answer, ApprovedBy, DataDriven) Values(@q, @a, @i, @d)";
                            break;
                        case "RejectedRules":
                            query = @"INSERT INTO RejectedRules (Question, Answer, RejectedBy, DataDriven) Values(@q, @a, @i, @d)";
                            break;
                        case "PendingRules":
                            if (!CheckExisting(question))
                            {
                                return false;
                            }
                            query = @"INSERT INTO PendingRules (Question, Answer, LastEditedBy, DataDriven) Values(@q, @a, @i, @d)";
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Unknown table");
                            return false;
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(new SqlParameter("q", question));
                    cmd.Parameters.Add(new SqlParameter("a", response));
                    cmd.Parameters.Add(new SqlParameter("i", user));
                    cmd.Parameters.Add(new SqlParameter("d", isDataDriven));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return true;
        }

        /*Edits an existing rule. Takes in the old question value to find the correct row,
         * the new question and answer for the question, the user who edited the rule
         * and the table the rule is in*/
        public void EditRule(string oldquestion, string question, string answer, string user, string table)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = "";
                    //Choose the correct table to edit
                    switch (table)
                    {
                        case "ApprovedRules":
                            query = @"UPDATE ApprovedRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @oq";
                            break;
                        case "RejectedRules":
                            query = @"UPDATE RejectedRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @oq";
                            break;
                        case "PendingRules":
                            query = @"UPDATE PendingRules SET Question = @q, Answer = @a, LastEditedBy = @i WHERE Question = @oq";
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine("Unknown table");
                            return;
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(new SqlParameter("q", question));
                    cmd.Parameters.Add(new SqlParameter("a", answer));
                    cmd.Parameters.Add(new SqlParameter("i", user));
                    cmd.Parameters.Add(new SqlParameter("oq", oldquestion));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /*Deletes a rule from a table. Takes the question of the rule to be deleted,
         * and the table that the rule is in*/
        public void DeleteRule(string question, string table)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = "";
                    //Choose the correct table
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
                            System.Diagnostics.Debug.WriteLine("Unknown table");
                            return;
                    }
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(new SqlParameter("q", question));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /*Approves a rule from the pendingrules table, and adds it to
         * the approvedrules table. Takes in the question of the rule 
         * to be approved, and the user who approved the rule*/
        public void ApproveRule(string question, string user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = @"SELECT Question, Answer, DataDriven FROM PendingRules WHERE Question = @q";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(new SqlParameter("q", question));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            DeleteRule(question, "PendingRules");
                            AddRule(rdr.GetString(0), rdr.GetString(1), user, "ApprovedRules", rdr.GetBoolean(2));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /*Rejects a rule from the pendingrules table, and adds it to
         * the rejectedrules table. Takes in the question of the rule 
         * to be rejected, and the user who rejected the rule*/
        public void RejectRule(string question, string user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = @"SELECT Question, Answer, DataDriven FROM PendingRules WHERE Question = @q";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add(new SqlParameter("q", question));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            DeleteRule(question, "PendingRules");
                            AddRule(rdr.GetString(0), rdr.GetString(1), user, "RejectedRules", rdr.GetBoolean(2));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /*Finds all of the rules in the pendingrules table and adds 
         * them to a DataTable so they can be printed to the screen
         * Returns a DataTable conaining all of the rules*/
        public DataTable PrintPendingRules()
        {
            DataTable ruleslist = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, LastEditedBy FROM PendingRules", conn);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Load the results into to the ruleslist DataTable
                        ruleslist.Load(rdr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Finds all of the rules in the approvedrules table and adds 
         * them to a DataTable so they can be printed to the screen
         * Returns a DataTable conaining all of the rules*/
        public DataTable PrintApprovedRules()
        {
            DataTable ruleslist = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, ApprovedBy, LastEditedBy FROM ApprovedRules", conn);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Load the results into to the ruleslist DataTable
                        ruleslist.Load(rdr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Finds all of the rules in the rejectedrules table and adds 
         * them to a DataTable so they can be printed to the screen
         * Returns a DataTable conaining all of the rules*/
        public DataTable PrintRejectedRules()
        {
            DataTable ruleslist = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, RejectedBy, LastEditedBy FROM RejectedRules", conn);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Load the results into to the ruleslist DataTable
                        ruleslist.Load(rdr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Takes in a question and returns the corresponding answer for that rule 
         * from the approvedrules table*/
        public string GetAnswer(string question)
        {
            //Remove extra whitespace and punctuation from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, @"(\p{P}+)(?=\Z|\r\n)", "");

            Dictionary<string, string> splitQuestion = SplitDataDrivenQuestion(question);

            string wildCard = "";
            if (splitQuestion != null && (wildCard = splitQuestion["wildCard"]) != null)
            {
                return GetDataDrivenAnswer(splitQuestion);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    //Query ignores case and punctuation when finding the answer
                    SqlCommand cmd = new SqlCommand(@"SELECT Answer FROM ApprovedRules WHERE LOWER(Question) LIKE @q", conn);
                    cmd.Parameters.Add(new SqlParameter("q", question.ToLower() + "%"));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            return rdr.GetString(0);
                        }
                        //Otherwise give information to the user that no answer was found
                        else
                        {
                            return "Sorry, no answer was found for that query.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return "Sorry, no answer was found for that query.";
        }

        public string GetDataDrivenAnswer(Dictionary<string, string> splitQuestion)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    //Query ignores case and punctuation when finding the answer
                    SqlCommand cmd = new SqlCommand(@"SELECT Answer FROM ApprovedRules WHERE LOWER(Question) = @q", conn);
                    string test = splitQuestion["prefix"].ToLower() + "[%]" + splitQuestion["sufix"].ToLower();
                    cmd.Parameters.Add(new SqlParameter("q", test));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            Dictionary<string, string> splitResponse = SplitAnswerCol(rdr.GetString(0));

                            GetAnswer2(splitQuestion, splitResponse);
                        }
                        //Otherwise give information to the user that no answer was found
                        else
                        {
                            return "Sorry, no answer was found for that query.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return "Sorry, no answer was found for that query.";
        }

        public string GetAnswer2(Dictionary<string, string> splitQuestion, Dictionary<string, string> splitResponse)
        {
            try
            {
                using (SqlConnection conn1 = new SqlConnection(path))
                {
                    conn1.Open();
                    //Query ignores case and punctuation when finding the answer
                    SqlCommand cmd;
                    if (splitResponse["lookup"] == "Genre") cmd = new SqlCommand(@"SELECT @a FROM DataDrivenRules WHERE LOWER(Genre) LIKE LOWER(@q)", conn1);
                    else cmd = new SqlCommand(@"SELECT @a FROM DataDrivenRules WHERE LOWER(MovieTitle) LIKE LOWER(@q)", conn1);
                    cmd.Parameters.Add(new SqlParameter("a", "Genre"));
                    cmd.Parameters.Add(new SqlParameter("q", "Room" + "%"));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            return rdr.GetString(0);
                        }
                        //Otherwise give information to the user that no answer was found
                        else
                        {
                            return "Sorry, no answer was found for that query.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return "Sorry, no answer was found for that query.";
        }

        public Dictionary<string, string> SplitAnswerCol(string answerCol)
        {
            string response = "";
            string lookup = "";
            string answer = "";
            int i = 0;

            foreach (Char c in answerCol)
            {
                if (c != '|')
                {
                    switch (i)
                    {
                        case 0:
                            response += c;
                            break;
                        case 1:
                            lookup += c;
                            break;
                        case 2:
                            answer += c;
                            break;
                    }
                }
                else i++;
            }

            return new Dictionary<string, string> {
                {"response", response },
                {"lookup", lookup },
                {"answer", answer }
            };
        }

        /* Gets the value of the wildcard. Grabs all DataDriven questions and converts them all into prefix/sufix.
         * All matching prefix/sufixes get stored, the rest ignored. It then picks the longest prefix & sufix combo
           to determine the best match. Remove this prefix and sufix from the original question and you have the wildcard. */
        public Dictionary<string, string> SplitDataDrivenQuestion(string question)
        {
            string prefix = "";
            string sufix = "";
            Dictionary<string, string> split = new Dictionary<string, string> {
                { "prefix", "" },
                { "wildCard","" },
                { "sufix", ""}};
            List<Tuple<string, string>> matchedWildCards = new List<Tuple<string, string>>();

            foreach (String s in GetDataDrivenQuestions())
            {
                bool metWildCard = false;

                foreach (Char c in s)
                {
                    if (c == '[' || c == '%' || c == ']')
                    {
                        metWildCard = true;
                    }
                    else if (!metWildCard)
                    {
                        prefix += c;
                    }
                    else if (metWildCard)
                    {
                        sufix += c;
                    }
                }

                prefix = Regex.Replace(prefix, "\\s+", " ");
                prefix = Regex.Replace(prefix, @"(\p{P}+)(?=\Z|\r\n)", "");
                prefix = prefix.ToLower();
                sufix = Regex.Replace(sufix, "\\s+", " ");
                sufix = Regex.Replace(sufix, @"(\p{P}+)(?=\Z|\r\n)", "");
                sufix = sufix.ToLower();
                question = question.ToLower();

                if (question.StartsWith(prefix) && question.EndsWith(sufix))
                {
                    matchedWildCards.Add(new Tuple<string, string>(prefix, sufix));
                }
                prefix = "";
                sufix = "";
            }

            if (matchedWildCards.Count >= 1)
            {

                string longestPrefix = "";
                string longestSufix = "";

                foreach (Tuple<string, string> t in matchedWildCards)
                {
                    if (t.Item1.Length > longestPrefix.Length) longestPrefix = t.Item1;
                    if (t.Item1.Length >= longestPrefix.Length && t.Item2.Length > longestSufix.Length) longestSufix = t.Item2; 
                }

                prefix = longestPrefix;
                sufix = longestSufix;
            }
            else if (matchedWildCards.Count == 0)
            {
                return null;
            }

            if (question.StartsWith(prefix) && question.EndsWith(sufix))
            {
                split["prefix"] = prefix;
                split["wildCard"] = question.Substring(prefix.Length, question.Length - prefix.Length - sufix.Length);
                split["sufix"] = sufix;
            }

            return split;
        }

        //Returns list of all data driven questions
        public List<string> GetDataDrivenQuestions()
        {
            List<string> dataRules = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT Question FROM ApprovedRules WHERE DataDriven = 1", conn);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //If an answer was found return that
                        while (rdr.Read())
                        {
                            dataRules.Add(rdr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return dataRules;
        }

        /*Finds all of the rules approved by a particular user and returns 
         * that list as a dictionary. Takes in a userID and returns a dictionary of the rules*/
        public Dictionary<string, string> PrintUsersApprovedRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, ApprovedBy FROM ApprovedRules WHERE ApprovedBy = @i", conn);
                    cmd.Parameters.Add(new SqlParameter("i", user.ConvertIdToUsername(userId)));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Loop through the results found and add them to the ruleslist dictionary
                        while (rdr.Read())
                        {
                            ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Finds all of the rules rejected by a particular user and returns 
         * that list as a dictionary. Takes in a userID and returns a dictionary of the rules*/
        public Dictionary<string, string> PrintUsersRejectedRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, RejectedBy FROM RejectedRules WHERE RejectedBy = @u", conn);
                    cmd.Parameters.Add(new SqlParameter("u", user.ConvertIdToUsername(userId)));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Loop through the results found and add them to the ruleslist dictionary
                        while (rdr.Read())
                        {
                            ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Finds all of the rules added to the pendingrules table by a particular user and returns 
         * that list as a dictionary. Takes in a userID and returns a dictionary of the rules*/
        public Dictionary<string, string> PrintUsersPendingRules(string userId)
        {
            User user = new User();
            Dictionary<string, string> ruleslist = new Dictionary<string, string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Question, Answer, LastEditedBy FROM PendingRules WHERE LastEditedBy = @i", conn);
                    cmd.Parameters.Add(new SqlParameter("i", user.ConvertIdToUsername(userId)));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Loop through the results found and add them to the ruleslist dictionery
                        while (rdr.Read())
                        {
                            ruleslist.Add(rdr.GetString(0), rdr.GetString(1));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return ruleslist;
        }

        /*Returns the number of approved rules.*/
        public int CountApproved()
        {
            return PrintApprovedRules().Rows.Count;
        }

        /*Returns the number of rejected rules.*/
        public int CountRejected()
        {
            return PrintRejectedRules().Rows.Count;
        }

        public string GetAnswerFromPending(string question)
        {
            //Remove extra whitespace and punctuation from the question
            question = Regex.Replace(question, "\\s+", " ").Trim();
            question = Regex.Replace(question, @"(\p{P}+)(?=\Z|\r\n)", "");

            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    //Query ignores case and punctuation when finding the answer
                    SqlCommand cmd = new SqlCommand(@"SELECT Answer FROM PendingRules WHERE LOWER(Question) LIKE @q", conn);
                    cmd.Parameters.Add(new SqlParameter("q", question.ToLower() + "%"));
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //If an answer was found return that
                        if (rdr.Read())
                        {
                            return rdr.GetString(0);
                        }
                        //Otherwise give information to the user that no answer was found
                        else
                        {
                            return "Sorry, no answer was found for that query.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return "Sorry, no answer was found for that query.";
        }

    }
}
