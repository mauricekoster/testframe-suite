
namespace ActionWord.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Win32;
    using System.Data.OleDb;
    using System.Reflection;
    using System.IO;
    using System.Diagnostics;

    using General.Presenter;
    
    public class TFDBManager
    {

        public class DatabaseNotFoundException : Exception { }
        private static string __current;
        private static string __dbfile;
        private static RegistryKey __databaseKey;

        public static string Current {
            get { return __current; }
            set {
                if (value == "")
                {
                    // unset the current
                    __current = "";
                    __dbfile = "";
                    __databaseKey.SetValue("Current", __current);
                    __databaseKey.SetValue("Filename", __dbfile);
                }
                else if (__databaseKey.GetValueNames().Any(s => s.Equals(value)))
                {
                    __current = value;
                    __dbfile = __databaseKey.GetValue(__current).ToString();
                    __databaseKey.SetValue("Current", __current);
                    __databaseKey.SetValue("Filename", __dbfile);
                }
                else
                    throw new DatabaseNotFoundException();
            }
        }
        public static string DatabaseFilename
        {
            get { return __dbfile;  }
        }

        static TFDBManager()
        {
            __current = "";
            __databaseKey = RegistrySupport.SubKey("Database");

            __current = __databaseKey.GetValue("Current", "").ToString();
            __dbfile = __databaseKey.GetValue("Filename", "").ToString();

        }

        /// <summary>
        /// Get an open connect, build from the database filename.
        /// </summary>
        /// <param name="dbfile">The database filename (Full path)</param>
        /// <returns>An open connection to the database.</returns>
        private static OleDbConnection GetConnection(string dbfile)
        {
            string connect_string;

            if (Environment.Is64BitProcess)
            {
                connect_string = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                   "Data Source=" + dbfile + ";" +
                   "Jet OLEDB:Database Password=dummy;Jet OLEDB:Engine Type=5"; // ;Jet OLEDB:Engine Type=5
            }
            else
            {
                connect_string = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                       "Data Source=" + dbfile + ";" +
                       "Jet OLEDB:Database Password=dummy;Jet OLEDB:Engine Type=5"; // ;Jet OLEDB:Engine Type=5
            }

            OleDbConnection _conn =
               new OleDbConnection(connect_string);

            _conn.Open();
        
            return _conn;
        }

        
        public static Dictionary<string,string> GetDatabases()
        {
            var dict = new Dictionary<string,string>();

            foreach (var k in __databaseKey.GetValueNames().Where(s => s != "Current" && s != "Filename"))
            { 
                var v = __databaseKey.GetValue(k).ToString();
                dict.Add(k, v);
            }
            

            return dict;
        }

        public static void NewDatabase(string shortname, string databaseFullFilename)
        {


            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("ActionWord.Presenter.Database.new.db"))
            using (Stream output = File.Create(databaseFullFilename,1024,FileOptions.WriteThrough))
            {
                input.CopyTo(output);
                output.Flush();
                output.Close();
            }

            try
            {
                OleDbConnection myConnection = GetConnection(databaseFullFilename);

                ExecuteEmbeddedSqlScript(myConnection, "Native.New.txt");
                myConnection.Close();

                // get reg key
                __databaseKey.SetValue(shortname, databaseFullFilename);
            }
            catch(Exception ex)
            {
                // something went wrong.
                Debug.WriteLine(ex.Message);
            }

        }


        public static void AddDatabase(string shortname, string databaseFullFilename)
        {
            // get reg key
            __databaseKey.SetValue(shortname, databaseFullFilename);
            
        }

        public static void RemoveDatabase(string shortname)
        {
            if (Current == shortname) Current = "";
            __databaseKey.DeleteValue(shortname);
        }

        public static void ExecuteEmbeddedSqlScript( OleDbConnection conn, string embedded_file)
        {
            OleDbCommand myCommand = new OleDbCommand();
            myCommand.Connection = conn;

            // open embedded stream
            var sql = new StringBuilder();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ActionWord.Presenter.Database." + embedded_file))
            {
                if (stream == null) throw new ArgumentException(  "embedded_file");

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        line = line.Trim();
                        if (line.StartsWith("--")) continue;

                        sql.AppendLine(line);
                        if (line.EndsWith(";"))
                        {
                            myCommand.CommandText = sql.ToString();
                            myCommand.ExecuteNonQuery();
                            sql.Clear();
                        }
                    }
                }
            }
        }
    }
}
