using System;
using System.Collections.Generic;

namespace BingVoiceSystem
{
    public class UserController
    {
        public List<User> users { get; }
        public User loggedInUser { get; set; }
        private int genId = 100001;

        /*
         * Constructor
         */
        public UserController()
        {
            //temp users
            users = new List<User>();
            users.Add(new User(genId++, "admin", "password", new List<Role>() { Role.DataMaintainer, Role.Editor, Role.Approver, Role.Registered }));
            users.Add(new User(genId++, "dataMaintainer", "password", new List<Role>() { Role.DataMaintainer, Role.Registered }));
            users.Add(new User(genId++, "editor", "password", new List<Role>() { Role.Editor, Role.Registered }));
            users.Add(new User(genId++, "approver", "password", new List<Role>() { Role.Approver, Role.Registered }));
            users.Add(new User(genId++, "registered", "password", new List<Role>() { Role.Registered }));
        }

        public void AddUser(string username, string password, List<Role> roles)
        {
            users.Add(new User(genId++, username, password, roles));
        }

        public void RemoveUser(string username)
        {
            foreach (User user in users)
            {
                if (user.username.Equals(username))
                {
                    users.Remove(user);
                }
            }
        }

        public User ValidateLogin(string username, string password)
        {
            foreach (User user in users)
            {
                if (user.CheckUserNameAndPassword(username, password))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
