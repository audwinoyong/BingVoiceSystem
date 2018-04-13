using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BingVoiceSystem
{
    public enum Role { DataMaintainer, Editor, Approver, Registered };

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
    }
}