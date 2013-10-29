using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.ComponentModel;

namespace ActionWord.Presenter
{
    public class Element : INotifyPropertyChanged
    {
        // properties
        // old values
        int _id;
        string _name;
        string _fullname;

        bool _isdirty = false;
        public bool Dirty { get { return _isdirty; } set { _isdirty = value; } }

        bool _isnew = false;

        public int ID { get { return _id; } set { _id = value; } } // read-only, only settable by contruction, database will not change!
        
        public string Name { 
            get { return _name; } 
            set {_isdirty = true; _name = value; Notify("Name"); } 
        }

        public string Fullname { 
            get { return _fullname;  }
            set { _isdirty = false; _fullname = value; Notify("Fullname"); } 
        }
        
        private int _parent = 0;
        public int Parent { get { return _parent; } set { _parent = value; _isdirty = true; Notify("Element"); } } // refers to element to which Actionword belongs (parent)

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

        /// <summary>
        /// Apply changes to the database
        /// </summary>
        private void Update(string field, object value)
        {
            var conn = TFDatabase.Active.Connection;
            
            var cmd = new OleDbCommand("UPDATE [element] SET [" + field + "]=@Value WHERE [element_id]=@ID", conn);
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@ID", _id);

            cmd.ExecuteNonQuery();

        }

        public List<Element> Elements
        {
            get
            {
                return Element.GetList(_id);
            }
        }
        public List<Actionword> Actionwords
        {
            get
            {
                return Actionword.GetList(this);
            }
        }

        public List<Screen> Screens
        {
            get
            {
                return Screen.GetList(this);
            }
        }

        public static List<Element> GetList(int parent = 0)
        {
            var list = new List<Element>();

            var conn = TFDatabase.Active.Connection;

            var cmd = new OleDbCommand("SELECT [element_id], [element_name], [element_fullname] FROM [element] WHERE [parent]=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", parent);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var el = new Element()
                {
                      ID = Convert.ToInt32(reader["element_id"].ToString())
                    , Name = reader["element_name"].ToString()
                    , Fullname = reader["element_fullname"].ToString()
                    , Parent = parent
                };
                el.Dirty = false;
                list.Add(el);
            }

            cmd.Dispose();

            return list;
        }


        public static Element New(Element parent = null, string name = "<new element>")
        {
            var element = new Element();
            
            element.ID = 0;
            element.Name = name;
            if (parent!=null)
                element.Parent = parent.ID;
            else
                element.Parent = 0;
            element._isnew = true;

            return element;
        }

        public void Remove()
        {

            if (_id == -1) return; // already removed

            foreach (var el in this.Elements)
            {
                el.Remove();
            }
            foreach (var aw in this.Actionwords)
            {
                aw.Remove();
            }
            foreach (var scr in this.Screens)
            {
                scr.Remove();
            }

            // remove from database
            var conn = TFDatabase.Active.Connection;
            var cmd = new OleDbCommand("DELETE FROM [element] WHERE [element_id]=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", _id);
            cmd.ExecuteNonQuery();

            _id = -1;
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
                var cmd = new OleDbCommand("INSERT INTO [element] ([element_name], [parent]) VALUES (@Name, @Parent)", conn);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Parent", Parent);
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
                var sql = new StringBuilder("UPDATE [element] SET ");
                sql.Append("element_name=?, ");
                sql.Append("element_fullname=?, ");
                sql.Append("parent=?, ");
                sql.Append("WHERE [element_id]=@ID");

                var cmd = new OleDbCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("element_name", Nv(Name));
                cmd.Parameters.AddWithValue("element_fullname", Nv(Fullname));
                cmd.Parameters.AddWithValue("parent", Parent);

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
