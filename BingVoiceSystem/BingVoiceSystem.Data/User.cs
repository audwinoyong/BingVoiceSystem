using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace BingVoiceSystem
{
    public enum Role { Guest, DataMaintainer, Editor, Approver, Registered };

    public class User
    {
        public int Id { get; }
        public string username { get; }
        private string password; //temp with bare security features

        List<Role> roles = new List<Role>();

        /* Constructor 
         * Optional parameters
         */
        public User(int id, string username, string password, List<Role> roles)
        {
            this.username = username;
            this.password = password;
            this.roles = roles;
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            if (currentPassword.Equals(password))
            {
                password = newPassword;
            }
        }

        public bool CheckPassword(string attempt)
        {
            if (attempt.Equals(password)) { return true; }
            else { return false; }
        }

        public bool CheckUserNameAndPassword(string username, string password)
        {
            return this.username.Equals(username) && this.password.Equals(password);
        }

        private string GetPath()
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            return path;
        }

        public List<string> GetUserByRole(Role role) 
        {
            List<string> usersList = new List< string>();

            using (SqlConnection conn = new SqlConnection(GetPath()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM AspNetUsers", conn);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (GetRole(rdr.GetString(0)).Contains(Role.Editor))
                        {
                            usersList.Add(rdr.GetString(0));
                        }
                    }
                }
            }

            return usersList;
        }

        public void SetRole(List<Role> roles)
        {
            int roleNum = 0;
            foreach (Role role in roles)
            {
                switch (role)
                {
                    case Role.Registered:
                        roleNum += 1;
                        break;
                    case Role.DataMaintainer:
                        roleNum += 2;
                        break;
                    case Role.Editor:
                        roleNum += 4;
                        break;
                    case Role.Approver:
                        roleNum += 8;
                        break;
                }
            }

            using (SqlConnection conn = new SqlConnection(GetPath()))
            {
                conn.Open();
                string query = @"INSERT INTO AspNetUsers (Role) Values(@r)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("r", roleNum));

                cmd.ExecuteNonQuery();
            }
        }

        public List<Role> GetRole(string userId)
        {
            List<Role> roles = new List<Role>();
            int role = 0;

            using (SqlConnection conn = new SqlConnection(GetPath()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Role FROM AspNetUsers WHERE Id = @i", conn);
                cmd.Parameters.Add(new SqlParameter("i", userId));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Int32.TryParse(rdr.GetString(0), out role);
                    }
                }
            }

            while (role >= 0)
            {
                if (role >= 8)
                {
                    role -= 8;
                    roles.Add(Role.Approver);
                }
                else if (role >= 4)
                {
                    role -= 4;
                    roles.Add(Role.Editor);
                }
                else if (role >= 2)
                {
                    role -= 2;
                    roles.Add(Role.DataMaintainer);
                }
                else if (role >= 1)
                {
                    role -= 1;
                    roles.Add(Role.Registered);
                }
                else if (role == 0 && roles.Count == 0)
                {
                    roles.Add(Role.Guest);
                }
            }

            return roles;
        }
    }
}