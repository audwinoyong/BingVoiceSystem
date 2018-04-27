﻿using System;
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
        List<Role> roles = new List<Role>();
        string path;

        public User()
        {
            path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\BingVoiceSystem.Data"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            path = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public List<string> GetUsersByRole(Role role) 
        {
            List<string> usersList = new List< string>();

            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserId, RoleId FROM AspNetUserRoles WHERE RoleId = @r", conn);
                cmd.Parameters.Add(new SqlParameter("r", role.ToString()));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        usersList.Add(rdr.GetString(0));
                    }
                }
            }

            return usersList;
        }
        
        public string GetUsernameFromId(string userId)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserName FROM AspNetUsers WHERE Id = @i", conn);
                cmd.Parameters.Add(new SqlParameter("i", userId));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return rdr.GetString(0);
                    }
                    else return "";
                }
            }
        }

        public string GetIdFromUserName(string userName)
        {
            using (SqlConnection conn = new SqlConnection(path))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id FROM AspNetUsers WHERE UserName = @u", conn);
                cmd.Parameters.Add(new SqlParameter("u", userName));
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return rdr.GetString(0);
                    }
                    else return "";
                }
            }
        }
    }
}