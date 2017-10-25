using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;

namespace RemoveSqlitePassword
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion; 

            var strTargetFileName = string.Empty;

            var strAssemblyFileName = System.IO.Path.GetFileName(assembly.Location);

            Console.WriteLine("");
            WriteTitle("===== Remove SQLite Password " + version + " =====");
            if (args.Length == 0)
            {
                Console.WriteLine("param 0 = file name [Optional - will look for the first *.SQLite3 file in current folder]");
                Console.WriteLine("param 1 = PW [Optional - can be set in config file]");
                Console.WriteLine("");
                WriteInfo("===== Examples: ===");
                Console.WriteLine("To remove PW from fullpath file:");
                WriteInfo(strAssemblyFileName + $@" ""C:\fullpath\file1.sqlite3"" ""mysuperpassword"" ");
                Console.WriteLine("");
                Console.WriteLine("To remove PW from local folder and load PW from config file:");
                WriteInfo(strAssemblyFileName + $@" ""file1.sqlite3""");

                Console.WriteLine("");
                Console.WriteLine("To remove PW from FIRST SQLite file in local folder and load PW from config file:");
                WriteInfo(strAssemblyFileName);
                Console.WriteLine("");

                var strCurrentDir = System.IO.Path.GetDirectoryName(assembly.Location);

                var lstFiles = System.IO.Directory.GetFiles(strCurrentDir, "*.SQLite3");
                if (lstFiles.Length > 0)
                {
                    strTargetFileName = lstFiles[0];
                }
                else
                {
                    WriteError("ERROR: unable to find any SQLite3 target file in current directory!");
                    return;
                }
            }
            else
            {
                // param0 = file name
                strTargetFileName = args[0];
            }

            // param1 = known pw (if param missing, will use pw from config)
            var strPw = string.Empty;
            if (args.Length > 1)
            {
                strPw = args[1];
            }
            else
            {
                // get current pw from config
                strPw = ConfigurationManager.AppSettings["pwToRemove"];
            }
            WriteTitle("===== Process Progress =====");
            Console.WriteLine("");
            WriteInfo("INFO: File to process:");
            Console.WriteLine(strTargetFileName);
            WriteInfo("INFO: PW length to use: " + strPw.Length + " chars");

            //var conn2 = new SQLiteConnection($"URI=file:{strFileName};Version=3;Password={strPw};");
            //var conn = new SQLiteConnection($"Data Source={strFileName};Password={strPw};");

            bool blnIsOk = true;
            try
            {
                // open db file to remove PW
                using (var conn = new SQLiteConnection($"Data Source={strTargetFileName};Password={strPw};"))
                {
                    WriteInfo("INFO: opening connection 1 to remove PW...");
                    conn.Open();

                    if (conn.State == ConnectionState.Open)
                    {
                        WriteInfo("INFO: Opened connection 1 successfully.");
                        WriteInfo("INFO: removing password...");
                        conn.ChangePassword("");
                    }
                    else
                    {
                        WriteError("ERROR: unable to open SQLite file - double check your PW!");
                    }
                    Console.WriteLine("INFO: closing connection 1...");
                    conn.Close();
                    //conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                blnIsOk = false;
                if (ex.Message == "file is encrypted or is not a database\r\nnot an error")
                {
                    WriteError("ERROR while removing PW: Password is incorrect, or, file is already with blank PW");
                }
                else
                {
                    WriteError("ERROR while removing PW:");
                    WriteError(ex.ToString());
                }
            }

            if (blnIsOk)
            {
                try
                {
                    using (var connWithoutPw = new SQLiteConnection($"Data Source={strTargetFileName};"))
                    {
                        WriteInfo("INFO: opening connection 2 to test blank PW...");
                        connWithoutPw.Open();

                        if (connWithoutPw.State == ConnectionState.Open)
                        {
                            WriteInfo("INFO: Opened connection 2 successfully - password removed successfully!");
                        }
                        else
                        {
                            WriteError("ERROR: connection 2 failed. double check your PW!");
                        }
                        connWithoutPw.Close();
                    }
                }
                catch (Exception ex)
                {
                    WriteError("ERROR while testing if PW removed successfully:");
                    WriteError(ex.ToString());
                }
            }

            if (args.Length == 0)
            {
                WriteInfo("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void WriteError(string strError)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(strError);
            Console.ResetColor();
        }

        private static void WriteInfo(string strInfo)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(strInfo);
            Console.ResetColor();
        }

        private static void WriteTitle(string strTitle)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(strTitle);
            Console.ResetColor();
        }
    }
}