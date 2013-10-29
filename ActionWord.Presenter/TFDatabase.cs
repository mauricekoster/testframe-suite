
namespace ActionWord.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.OleDb;
    using System.Data;
    using System.Diagnostics;

    using General.Presenter;

    public sealed class TFDatabase
    {        
        static TFDatabase _tfdb ;
        private static string database_verion = "2009.07";
        public const string AppName = "TestFrame Toolbar";

        Settings __settings;
        OleDbConnection __conn;

        static TFDatabase()
        {
            _tfdb = null;
        }

        public TFDatabase(string dbfile)
        {
            __conn = GetConnection(dbfile);


        }
                
        public static bool IsActive
        {
            get { 
                if (_tfdb == null) return false;
                if (_tfdb.Connection == null) return false;
                return true;
            }
        }

        public static TFDatabase Active
        {
            get { return _tfdb; }
        }

        public OleDbConnection Connection
        {
            get { return __conn; }
        }

        public Settings Settings
        {
            get { 
                if (!IsActive) return null;
                if (__settings == null) __settings = new Settings(_tfdb);
                return __settings;
            }
        }

        

        private static OleDbConnection GetConnection(string dbfile)
        {
            string connect_string;
            Debug.WriteLine("Try opening connection to: " + dbfile);
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

            try
            {
                _conn.Open();
                Debug.WriteLine("Connection opened to: " + dbfile);
                
            }
            catch (Exception)
            {
                Debug.WriteLine("Connection failed!: " + dbfile);
                TFDBManager.Current = "";
                _conn = null;
            }
            return _conn;
        }

        public static void Open()
        {
            // must be closed first!
            if (IsActive) { return;  }

            string dbfile = TFDBManager.DatabaseFilename;
            if (dbfile == "") return;
            
            _tfdb = new TFDatabase(dbfile);

            if (!_tfdb.Settings.DatabaseVersion.Equals(database_verion))
            {
                UpgradeDatabase();
            }

        }

        public void Close()
        {
            if (__conn != null) 
            {
                Debug.WriteLine("Closing connection...:" + __conn.Database);
                __conn.Close();
                __conn.Dispose();
                __conn = null;
            }
        }
       
        private static void UpgradeDatabase()
        {
            Debug.WriteLine("Start upgrading to " + database_verion);
            while (!_tfdb.Settings.DatabaseVersion.Equals(database_verion))
            {
                string v = _tfdb.Settings.DatabaseVersion;
                string f = "Native." + v + ".txt";

                Debug.WriteLine("Upgrading from version: " + v);

                try
                {
                    TFDBManager.ExecuteEmbeddedSqlScript(_tfdb.__conn, f);
                }
                catch (ArgumentException ex)
                {
                    throw new Exception("Unknown upgrade path. Cannot convert database from old version " + v + " to version " + database_verion
                        + ". Please contact maintainer for support! Maybe someone converted using a newer TestFrame Toolbar?" 
                        , ex);
                }
                

                // force rereading settings...
                _tfdb.__settings = null;

            }
            Debug.WriteLine("Done upgrading.");
        }
    }
}
