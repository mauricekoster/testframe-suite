
namespace Configuration.Module
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    using ActionWord.Presenter;

    /// <summary>
    /// Interaction logic for GeneralOptions.xaml
    /// </summary>
    public partial class GeneralOptions : Window
    {
        public GeneralOptions()
        {
            InitializeComponent();
            this.DataContext = TFDatabase.Active.Settings;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel) return;

            TFDatabase.Active.Settings.TemplateDirectory = dialog.SelectedPath;
            
        }
    }
}
