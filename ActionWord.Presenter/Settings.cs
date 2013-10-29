using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel;

namespace ActionWord.Presenter
{
    public class Settings : INotifyPropertyChanged
    {
        private TFDatabase _tfdb;
        private DataTable _tbl;
        private Dictionary<string, string> _settings;

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged implementation

        #region Constructors
        
        public Settings()
        {
            _settings = new Dictionary<string, string>();
        }

        public Settings(TFDatabase _tfdb)
        {
            this._tfdb = _tfdb;
            _settings = new Dictionary<string, string>();

            OleDbCommand selectCMD = new OleDbCommand("SELECT * FROM Setting", _tfdb.Connection);
            selectCMD.CommandTimeout = 30;

            OleDbDataAdapter custDA = new OleDbDataAdapter();
            custDA.SelectCommand = selectCMD;

            _tbl = new DataTable();
            custDA.Fill(_tbl);
            _settings = _tbl.AsEnumerable()
                .ToDictionary(r => r.Field<string>("setting"), r => r.Field<string>("value"));

        }
        #endregion

        public string DatabaseVersion
        {
            get { return this["database version"]; }
            
        }

        #region Actionword properties
       
        public bool AllowUppercaseInActionword
        {
            get { return (this["allow uppercase in actionword"].ToLower().Equals("yes") ? true : false); }
            set { 
                if (value == true) 
                    this["allow uppercase in actionword"] = "Yes";
                else
                    this["allow uppercase in actionword"] = "No";
                Notify("AllowUppercaseInActionword");
            }
        }
        

        public int ArgumentsPerRow
        {
            get { 
                try
                {
                    return Convert.ToInt32(this["arguments per row"]);
                }
                catch(Exception)
                {
                    return 0;
                }
            }
            set { 
                this["arguments per row"] = value.ToString();
                Notify("ArgumentsPerRow");
            }
        }

        #endregion

        #region Template properties

        public string TemplateDirectory
        {
            get { return this["template directory"]; }
            set
            {   
                this["template directory"] = value;
                Notify("TemplateDirectory");
            }
        }
        #endregion

        #region Phase testing properties

        public bool PhaseTesting
        {
            get { return (this["phase testing"].ToLower().Equals("true") ? true : false); }
            set
            {
                if (value == true)
                    this["phase testing"] = "True";
                else
                    this["phase testing"] = "False";
                Notify("PhaseTesting");
            }
        }

        public string PhaseDefault
        {
            get { return this["phase default"]; }
            set
            {
                this["phase default"] = value;
                Notify("PhaseDefault");
            }
        }
        #endregion

        #region Support functions


        /// <summary>
        /// Settings using a indexer. Workhorse for the properties.
        /// </summary>
        /// <param name="setting">Settingname</param>
        /// <returns>Value as string.</returns>
        public string this[string setting]
        {
            get
            {

                if (_settings.Count() == 0) return "";
                if (!_settings.Keys.Any(k => k.Equals(setting))) return "";

                return _settings[setting];
            }
            set
            {

                if (TFDatabase.IsActive)
                {
                    if (!_settings.Keys.Any(k => k.Equals(setting)))
                    {
                        // create setting in database
                        OleDbCommand cmd = new OleDbCommand("insert into [setting] ([setting], [value]) values (@setting,@value)", _tfdb.Connection);
                        cmd.Parameters.AddWithValue("@setting", setting);
                        cmd.Parameters.AddWithValue("@value", value);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // update setting in database
                        OleDbCommand cmd = new OleDbCommand("update [setting] set [value] = @value where [setting] = @setting", _tfdb.Connection);
                        cmd.Parameters.AddWithValue("@value", value);
                        cmd.Parameters.AddWithValue("@setting", setting);

                        cmd.ExecuteNonQuery();
                    }
                }
                
                _settings[setting] = value;
            }
        }
        #endregion

    } // end of class
        
}
