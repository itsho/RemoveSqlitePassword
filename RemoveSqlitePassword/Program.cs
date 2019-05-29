using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RemoveSqlitePassword
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fileVersionInfo.ProductVersion;

            var lstTargetFiles = new List<string>();

            var strAssemblyFileName = System.IO.Path.GetFileName(assembly.Location);

            Log.WriteRegular("");
            Log.WriteInfo($"===== Remove SQLite Password {version} =====", true);
            if (args.Length == 0)
            {
                WriteIntro(strAssemblyFileName);

                var strCurrentDir = System.IO.Path.GetDirectoryName(assembly.Location);

                var lstFiles1 = System.IO.Directory.GetFiles(strCurrentDir, "*.SQLite3");
                var lstFiles2 = System.IO.Directory.GetFiles(strCurrentDir, "*.SQLite");
                var lstFiles = lstFiles1.Concat(lstFiles2).ToList();

                if (lstFiles.Count > 0)
                {
                    lstTargetFiles.AddRange(lstFiles);
                }
                else
                {
                    Log.WriteError("ERROR: unable to find any target files (*.SQLite|*.SQLite3) in current directory!");
                    return;
                }
            }
            else
            {
                // param0 = file name
                lstTargetFiles.Add(args[0]);
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

            Log.WriteInfo("===== Process Progress =====", true);
            Log.WriteRegular("");

            Log.WriteInfo("INFO: PW length to use: " + strPw.Length + " chars");

            foreach (var strTargetFile in lstTargetFiles)
            {
                Log.WriteInfo($"INFO: Processing file: '{Path.GetFileName(strTargetFile)}'...", true);

                Log.WriteInfo("INFO: trying to remove password...");
                if (Logic.RemovePassword(strTargetFile, strPw))
                {
                    Log.WriteInfo("INFO: password removed successfully.");
                }
                else
                {
                    Log.WriteError("ERROR: Failed to remove password.");
                }

                Log.WriteInfo("INFO: Checking if file has blank password...");
                if (Logic.IsFileHasBlankPassword(strTargetFile))
                {
                    Log.WriteInfo("INFO: file can now be accessed with blank password.");
                }
                else
                {
                    Log.WriteError("ERROR: unable to access the file with blank password.");
                }
            }

            if (args.Length == 0)
            {
                Log.WriteInfo("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void WriteIntro(string strAssemblyFileName)
        {
            Log.WriteRegular("param 0 = File Name [Optional - will look for the all *.SQLite|*.SQLite3 files in current folder]");
            Log.WriteRegular("param 1 = PW [Optional - can be set in config file]");
            Log.WriteRegular("");
            Log.WriteInfo("===== Examples: ===");
            Log.WriteRegular("To remove PW from fullpath file:");
            Log.WriteInfo($@"{strAssemblyFileName} ""C:\fullpath\file1.sqlite3"" ""mysuperpassword"" ");
            Log.WriteRegular("");
            Log.WriteRegular("To remove PW from local folder and load PW from config file:");
            Log.WriteInfo($@"{strAssemblyFileName} ""file1.sqlite3""");

            Log.WriteRegular("");
            Log.WriteRegular("To remove PW from *.SQLite3 files in local folder and load PW from config file:");
            Log.WriteInfo(strAssemblyFileName);
            Log.WriteRegular("");
        }
    }
}