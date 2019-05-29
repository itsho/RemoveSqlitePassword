using System;
using System.Data;
using System.Data.SQLite;

namespace RemoveSqlitePassword
{
    internal class Logic
    {
        public static bool RemovePassword(string strTargetFile, string strPw)
        {
            var isOk = false;
            try
            {
                // open db file to remove PW
                using (var conn = new SQLiteConnection($"Data Source={strTargetFile};Password={strPw};"))
                {
                    Log.WriteInfo("INFO: opening connection to remove PW...");
                    conn.Open();

                    if (conn.State == ConnectionState.Open)
                    {
                        Log.WriteInfo("INFO: Opened connection successfully.");
                        Log.WriteInfo("INFO: removing password...");
                        conn.ChangePassword("");
                    }
                    else
                    {
                        Log.WriteError("ERROR: unable to open SQLite file - double check your PW!");
                    }
                    Log.WriteRegular("INFO: closing connection...");
                    conn.Close();
                }

                isOk = true;
            }
            catch (SQLiteException ex)
            {
                isOk = false;
                if (ex.ResultCode == System.Data.SQLite.SQLiteErrorCode.NotADb) //  ex.Message == "file is encrypted or is not a database\r\nnot an error")
                {
                    Log.WriteError("ERROR: File is already with blank PW, or, given password is incorrect");
                }
                else
                {
                    Log.WriteError("ERROR: Unable to remove PW - SQLiteException:");
                    Log.WriteError(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                isOk = false;

                Log.WriteError("ERROR: unable to remove PW:");
                Log.WriteError(ex.ToString());
            }

            return isOk;
        }

        public static bool IsFileHasBlankPassword(string strTargetFile)
        {
            try
            {
                using (var connWithoutPw = new SQLiteConnection($"Data Source={strTargetFile};"))
                {
                    Log.WriteInfo("INFO: opening connection to test blank PW...");
                    connWithoutPw.Open();

                    if (connWithoutPw.State == ConnectionState.Open)
                    {
                        Log.WriteInfo("INFO: Opened connection with blank password successfully!");
                    }
                    else
                    {
                        Log.WriteError("ERROR: connection with blank password failed!");
                    }
                    connWithoutPw.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError("ERROR while checking if file has blank password:");
                Log.WriteError(ex.ToString());
                return false;
            }
        }
    }
}