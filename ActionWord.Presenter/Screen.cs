using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace ActionWord.Presenter
{
    public class Screen
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public int ElementID { get; set; }
        public Element Element { get; set; }

        public static List<Screen> GetList(Element element = null)
        {
            var list = new List<Screen>();
            var conn = TFDatabase.Active.Connection;
            OleDbCommand cmd;

            if (element == null)
            {
                cmd = new OleDbCommand("SELECT * FROM [screen]", conn);
            }
            else
            {
                cmd = new OleDbCommand("SELECT * FROM [screen] WHERE [element]=@ID", conn);
                cmd.Parameters.AddWithValue("@ID", element.ID);
            }

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Screen()
                {
                    ID = Convert.ToInt32(reader["screen_id"].ToString())
                    ,
                    Name = reader["screenname"].ToString()
                    ,
                    Fullname = reader["screenname full"].ToString()
                    ,
                    ElementID = Convert.ToInt32(reader["element"].ToString())
                    ,
                    Element = element
                });
            }

            cmd.Dispose();
            return list;
        }


        internal void Remove()
        {
            throw new NotImplementedException();
        }
    } // end of class
}
