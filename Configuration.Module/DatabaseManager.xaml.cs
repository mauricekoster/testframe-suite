
namespace Configuration.Module
{

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    using System.Threading.Tasks;
    using System.Data.OleDb;

    using Microsoft.VisualBasic;

    using ActionWord.Presenter;
    using General.Presenter;

    /// <summary>
    /// Interaction logic for DatabaseManager.xaml
    /// </summary>
    public partial class DatabaseManager : Window
    {

        private const string AppName = "Database manager";

        private IDictionary<string, string> __list;

        public DatabaseManager()
        {
            InitializeComponent();

            FillList();
        }

        private void FillList()
        {
            __list = TFDBManager.GetDatabases();

            listDatabases.Items.Clear();
            foreach (var i in __list)
            {
                int idx = listDatabases.Items.Add(i.Key);
            }

            listDatabases.SelectedItem = TFDBManager.Current;
        }

       
        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
           
            // Create OpenFileDialog
            var dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".mdb";
            dlg.Filter = "TestFrame Databases (.mdb)|*.mdb";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
         
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string shortname = Globals.InputBox("Enter shortname for database:");
                Parallel.Invoke( () => {
                        TFDBManager.NewDatabase(shortname, filename);
                    });

                FillList();
                listDatabases.SelectedValue = shortname;

            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

            if (listDatabases.SelectedItem != null)
            {
                TFDBManager.Current = listDatabases.SelectedItem.ToString();
            }

            this.Close();
        }

        private void listDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listDatabases.SelectedItem != null)
            {
                var k = listDatabases.SelectedItem.ToString();
                txtSelectedDatabasePath.Text = __list[k];
            }
        }

        private void btnAddExisting_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".mdb";
            dlg.Filter = "TestFrame Databases (.mdb)|*.mdb";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string shortname = Globals.InputBox("Enter shortname for database:");
                TFDBManager.AddDatabase(shortname, filename);
                FillList();
                listDatabases.SelectedValue = shortname;
            }
        }

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            string shortname = listDatabases.SelectedItem.ToString();
            string connect_string;

            if (Environment.Is64BitProcess)
            {
                connect_string = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                   "Data Source=" + __list[shortname] + ";" +
                   "Jet OLEDB:Database Password=dummy;Jet OLEDB:Engine Type=5"; // ;Jet OLEDB:Engine Type=5
            }
            else
            {
                connect_string = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                       "Data Source=" + __list[shortname] + ";" +
                       "Jet OLEDB:Database Password=dummy;Jet OLEDB:Engine Type=5"; // ;Jet OLEDB:Engine Type=5
            }

            OleDbConnection myConnection =
               new OleDbConnection(connect_string);
            try
            {
                myConnection.Open();
                MessageBox.Show("Connection opened!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection failed! " + ex.Message);
            }

            myConnection.Close();
        }

       
    }
}
