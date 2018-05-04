using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BingVoiceSystem
{
    public class MovieData
    {
        public string path;

        /*Constructor: creates and sets the path to the database*/
        public MovieData()
        {
            path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }

        public bool CheckExisting(string movieTitle)
        {
            //Remove punctuation
            movieTitle = Regex.Replace(movieTitle, "[^\\w\\s]", "");
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    string query = @"SELECT MovieTitle FROM DataDrivenData";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    //Check if the question exists in the pendingrules table
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string response = Regex.Replace(rdr.GetString(0), "[^\\w\\s]", "");
                            if (movieTitle.ToLower().Equals(response.ToLower()))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return true;
        }

        /*Adds a new data to the appropriate table. Takes in a movie Title and genre for the data and
         * a user for who created the data*/
        public bool AddData(string movieTitle, string genre, string user)
        {
            //Remove extra whitespace from the question
            movieTitle = Regex.Replace(movieTitle, "\\s+", " ").Trim();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"INSERT INTO DataDrivenData (MovieTitle, Genre, LastEditedBy) Values(@m, @g, @i)", conn);
                    cmd.Parameters.Add(new SqlParameter("m", movieTitle));
                    cmd.Parameters.Add(new SqlParameter("a", genre));
                    cmd.Parameters.Add(new SqlParameter("i", user));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return true;
        }

        /*Edits an existing data entry. Takes in the old movie value to find the correct row,
         * the new movie and genre for the movie, the user who edited the data*/
        public void EditData(string oldMovie, string movieTitle, string genre, string user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"UPDATE DataDrivenData SET MovieTitle = @m, Genre = @g, LastEditedBy = @i WHERE MovieTitle = @om", conn);
                    cmd.Parameters.Add(new SqlParameter("m", movieTitle));
                    cmd.Parameters.Add(new SqlParameter("g", genre));
                    cmd.Parameters.Add(new SqlParameter("i", user));
                    cmd.Parameters.Add(new SqlParameter("om", oldMovie));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        /*Deletes a data entry from a table. Takes the movie title of the data to be deleted*/
        public void DeleteData(string movieTitle)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"DELETE FROM DataDrivenData WHERE MovieTitle = @m", conn);
                    cmd.Parameters.Add(new SqlParameter("m", movieTitle));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public DataTable PrintData()
        {
            DataTable datalist = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(path))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT MovieTitle, Genre, LastEditedBy FROM DataDrivenData", conn);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        //Load the results into to the ruleslist DataTable
                        datalist.Load(rdr);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            return datalist;
        }
    }
}
