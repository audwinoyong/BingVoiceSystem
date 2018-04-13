using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BingVoiceSystem.Web
{
    public class main
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Hello");

            UserController userController = new UserController();

            Rules ruleslist = new Rules();
            ruleslist.PrintRules();

            Menu menu = new Menu(userController, ruleslist);
            menu.Show();
        }




        private bool False()
        {
            return false;
        }

        private int Int()
        {
            return 0;
        }

    }
}