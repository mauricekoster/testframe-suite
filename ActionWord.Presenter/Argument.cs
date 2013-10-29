namespace ActionWord.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.OleDb;
    using System.ComponentModel;

    /// <summary>
    /// Class representing an actionword argument.
    /// </summary>
    public class Argument : INotifyPropertyChanged
    {
        int _id;
        public int ID { get { return _id; } set { _id = value; } }
        int _nr = -1;
        public int Number { 
            get { return _nr; }
            set { if (_nr != -1 && _nr != value) Update("argument_number", value); _nr = value; Notify("Number"); ; }
        }
        string _name;
        public string Name
        {
            get { return _name; }
            set { if (_name != null && !_name.Equals(value)) Update("name", value); _name = value; Notify("Name"); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { if (_description != null && !_description.Equals(value)) Update("description", value); _description = value; Notify("Description"); }
        }
        private string _default;
        public string DefaultValue
        {
            get { return _default; }
            set { if (_default != null && !_default.Equals(value)) Update("default_value", value); _default = value; Notify("DefaultValue"); }
        }
        private bool _required = false;
        public bool Required {
            get { return _required; }
            set { if (_required != value) Update("required", value); _required = value; Notify("Required"); }
        }
        public bool HasValidation { get { return (ValidationID != 0); } }
        public int ValidationID { get; set; }

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

            var cmd = new OleDbCommand("UPDATE [argument] SET [" + field + "]=@Value WHERE [argument_id]=@ID", conn);
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.Parameters.AddWithValue("@ID", _id);
            cmd.ExecuteNonQuery();

        }

        // TODO other members of argument

        public Argument() { }

        public static List<Argument> GetList(int actionword_id)
        {
            var conn = TFDatabase.Active.Connection;
            var list = new List<Argument>();

            var cmd = new OleDbCommand("SELECT *" +
                " FROM [argument] WHERE [actionword_id]=@ID ORDER BY [argument_number]", conn);
            cmd.Parameters.AddWithValue("@ID", actionword_id);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //  [argument_id], [argument_number], [name], [description], [default_value], [required], [validation_id]
                list.Add( new Argument() {
                    ID = Convert.ToInt32(reader["argument_id"].ToString()),
                    Number = Convert.ToInt32( reader["argument_number"].ToString() ),
                    Name = reader["name"].ToString() ,
                    Description = reader["description"].ToString(),
                    DefaultValue = reader["default_value"].ToString(),
                    Required = reader.GetBoolean(reader.GetOrdinal("required")),
                    ValidationID = Convert.ToInt32(reader["validation_id"].ToString())
                });
            }

            cmd.Dispose();
            return list;
        }


        public static Argument New(Actionword actionword, string name)
        {
            var argument = new Argument();
            var conn = TFDatabase.Active.Connection;

            int arg_nr = actionword.Arguments.Count + 2;

            var cmd = new OleDbCommand("INSERT INTO [argument] ([actionword_id], [argument_number], [name]) VALUES (@Parent, @Number, @Name)", conn);
            
            cmd.Parameters.AddWithValue("@Parent", actionword.ID);
            cmd.Parameters.AddWithValue("@Number", arg_nr);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.ExecuteNonQuery();

            cmd = new OleDbCommand("SELECT @@IDENTITY AS IDVal", conn);
            object o = cmd.ExecuteScalar();

            var id = Convert.ToInt32(o.ToString());

            argument.ID = id;
            argument.Name = name;
            argument.Number = arg_nr;
            argument.Actionword = actionword;
            return argument;
        }

        internal void Remove()
        {

            if (HasValidation)
            {
                // TODO remove validation from argument
            }

            // remove from database
            var conn = TFDatabase.Active.Connection;
            var cmd = new OleDbCommand("DELETE FROM [argument] WHERE [argument_id]=@ID", conn);
            cmd.Parameters.AddWithValue("@ID", _id);
            cmd.ExecuteNonQuery();

            _id = -1;
            
        }

        public Actionword Actionword { get; set; }
    }
}
