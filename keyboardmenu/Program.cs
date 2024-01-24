using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.Media;

namespace keyboardmenu
{
    class app
    {
         static void Main(string[] args)    
        {
            Thread titleThread = new Thread(() =>
            {
                Random rndpwd = new Random();
                string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@*/?";
                StringBuilder res = new StringBuilder();
                while (true)
                {
                    for (int i = 0; i < 20; i++)

                    {
                        res.Append(valid[rndpwd.Next(valid.Length)]);
                    }

                    Title = res.ToString();
                    res.Clear();
                    Thread.Sleep(50); // delay
                }
            });


            titleThread.Start();

            while (true)
            {
                Application application = new Application();
                application.Start();
            }
        }
    }
}