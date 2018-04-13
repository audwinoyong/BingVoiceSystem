using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BingVoiceSystem.Web
{
    public class Menu
    {
        private UserController userController;
        private Rules rules;

        public Menu(UserController userController, Rules rules)
        {
            this.userController = userController;
            this.rules = rules;
        }

        public void Show()
        {
            Console.WriteLine("Welcome to Bing Voice System");
            bool quit = false;
            while (!quit)
            {
                Console.WriteLine("Choose options:" + Environment.NewLine +
                                  "A) Login" + Environment.NewLine +
                                  "B) Change password" + Environment.NewLine +
                                  "C) Add conversational rule" + Environment.NewLine
                                 );
                switch (GetKey())
                {
                    case 'a':
                    case 'A':
                        //Login
                        string username = GetValue("Username");
                        string password = GetValue("Password");

                        User validUser = userController.ValidateLogin(username, password);
                        if (validUser != null)
                        {
                            userController.loggedInUser = validUser;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect username or password.");
                        }
                        break;
                    case 'b':
                    case 'B':
                        //Change password
                        string currentPassword = GetValue("Current Password");
                        string newPassword = GetValue("New Password");

                        userController.loggedInUser.ChangePassword(currentPassword, newPassword);
                        break;
                    case 'c':
                    case 'C':
                        //Add conversational rule
                        string question = GetValue("Question");
                        string response = GetValue("Response");

                        rules.AddRule(question, response);
                        break;
                    default:
                        Console.WriteLine("Invalid key. Please try again.");
                        break;
                }
            }
            Console.WriteLine("Program closed.");
        }

        private char GetKey()
        {
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine();
            return key;
        }

        private string GetValue(string description)
        {
            Console.Write("{0}: ", description);
            return Console.ReadLine();
        }
    }
}