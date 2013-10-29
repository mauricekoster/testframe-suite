using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.ComponentModel;

namespace ActionWord.Presenter
{
    public class Actionword : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _description;
        private string _actions;
        private bool _isnew = false;

        private bool _isdirty = false;
        public bool Dirty { get { return _isdirty; } set { _isdirty = value; } }

        public int ID { get { return _id; } set { _id = value; } }

        public string Name {
            get { return _name; }
            set { _isdirty=true; _name = value; Notify("Name");   }
        }
        public string Description
        {
            get { return _description; }
            set { _isdirty = true; _description = value; Notify("Description"); }
        }
        public string Actions
        {
            get { return _actions; }
            set { _isdirty = true; _actions = value; Notify("Actions"); }
        }
        public string PreCondition { get; set; }
        public string PostCondition { get; set; }

        private int? _startscreen = null;
        public int? StartScreen {
            get { return _startscreen; }
            set { _isdirty = true; _startscreen = value; Notify("StartScreen"); }
        }
        private int? _endscreen = null;
        public int? EndScreen
        {
            get { return _endscreen; }
            set { _isdirty = true; _endscreen = value; Notify("EndScreen"); }
        }  // TODO Screen class

        private Element _element = null;
        public Element Element { get { return _element; } set { _element = value; _isdirty = true; Notify("Element"); } } // refers to element to which Actionword belongs (parent)

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


        public List<Argument> Arguments
        {
            get 
            {
                return Argument.GetList(ID);
            }
        }

        public Actionword()
        {
        }
        
        public static List<Actionword> GetList(Element element = null)
        {
            var list = new List<Actionword>();
            var conn = TFDatabase.Active.Connection;
            OleDbCommand cmd;

            if (element == null)
            {
                cmd = new OleDbCommand("SELECT * FROM [actionword]", conn);
            }
            else
            {
                cmd = new OleDbCommand("SELECT * FROM [actionword] WHERE [element]=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", element.ID);
            }

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var aw = new Actionword() {
                    ID = Convert.ToInt32(reader["actionword_ID"].ToString())
                    , Name = reader["actionword"].ToString()
                    , Description = reader["description"].ToString()
                    , Actions = reader["actions"].ToString()
                    , PreCondition = reader["precondition"].ToString()
                    , PostCondition = reader["postcondition"].ToString()
                    , Element = element
                };
                aw.Dirty = false;
                if ( reader["start_screen"]==null ) 
                    aw.StartScreen = int.Parse(reader["start_screen"].ToString());

                if (reader["end_screen"] == null)
                    aw.EndScreen = int.Parse(reader["end_screen"].ToString());

                list.Add(aw);
            }

            cmd.Dispose();
            return list;
        }

        /// <summary>
        /// Apply changes to the database
        /// </summary>
        private void Update(string field, object value)
        {
            var conn = TFDatabase.Active.Connection;

            var cmd = new OleDbCommand("UPDATE [actionword] SET [" + field + "]=@Value WHERE [actionword_ID]=@ID", conn);
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@ID", _id);

            cmd.ExecuteNonQuery();

        }

        public static Actionword GetActionword(string actionword)
        {
            return Actionword.GetList().Where(a => a.Name == actionword).First();
        }

        public void Remove()
        {
            if (_id == -1) return; // already removed

            foreach (var arg in this.Arguments)
            {
                arg.Remove();
            }
            
            // remove from database
            var conn = TFDatabase.Active.Connection;
            var cmd = new OleDbCommand("DELETE FROM [actionword] WHERE [actionword_id]=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", _id);
            cmd.ExecuteNonQuery();

            _id = -1;
        }

        public static Actionword New(Element element, string name)
        {
            var actionword = new Actionword() {  Element = element, Name = name };
            actionword._isnew = true;
            return actionword;
        }

        public void Save()
        {
            if (!_isdirty) return;

            if (_isnew) InsertNew();

            UpdateAll();
        }

        internal void InsertNew()
        {
            if (!_isnew) return;

            var conn = TFDatabase.Active.Connection;

            //TODO actionword must be unique, so if error do something
            try
            {
                var cmd = new OleDbCommand("INSERT INTO [actionword] ([actionword], [element]) VALUES (@Name, @Parent)", conn);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Parent", Element.ID);
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                
                throw;
            }

            var cmd2 = new OleDbCommand("SELECT @@IDENTITY AS IDVal", conn);
            object o = cmd2.ExecuteScalar();

            var id = int.Parse(o.ToString());
            ID = id;
            _isnew = false;

        }

        private void UpdateAll()
        {
            if (_isdirty)
            {
                var conn = TFDatabase.Active.Connection;
                var sql = new StringBuilder("UPDATE [actionword] SET ");
                sql.Append("actionword=?, ");
                sql.Append("description=?, ");
                sql.Append("actions=?, ");
                sql.Append("precondition=?, ");
                sql.Append("postcondition=?, ");
                sql.Append("start_screen=?, ");
                sql.Append("end_screen=?, ");
                sql.Append("element=? ");
                sql.Append("WHERE [actionword_ID]=@ID");

                var cmd = new OleDbCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("actionword", Nv(Name) );
                cmd.Parameters.AddWithValue("description", Nv(Description) );
                cmd.Parameters.AddWithValue("actions", Nv(Actions) );
                cmd.Parameters.AddWithValue("precondition", Nv(PreCondition) );
                cmd.Parameters.AddWithValue("postcondition", Nv(PostCondition) );
                cmd.Parameters.AddWithValue("start_screen", Nv(StartScreen) );
                cmd.Parameters.AddWithValue("end_screen", Nv(EndScreen) );
                cmd.Parameters.AddWithValue("element", Element.ID);

                cmd.Parameters.AddWithValue("@ID", ID);

                cmd.ExecuteNonQuery();

                _isdirty = false;
            }
        }

        private object Nv(object o)
        {
            if (o == null) return DBNull.Value;
            return o;
        }

        
    }
}
