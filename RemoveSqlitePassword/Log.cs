using System;

namespace RemoveSqlitePassword
{
    public class Log
    {
        public static void WriteError(string strError)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(strError);
            Console.ResetColor();
        }

        public static void WriteRegular(string str)
        {
            Console.WriteLine(str);
        }

        public static void WriteInfo(string strInfo, bool isTitle = false)
        {
            if (isTitle)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            Console.WriteLine(strInfo);
            Console.ResetColor();
        }
    }
}