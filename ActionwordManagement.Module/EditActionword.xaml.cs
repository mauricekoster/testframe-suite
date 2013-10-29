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
using General.Presenter;
using System.Collections.ObjectModel;

namespace ActionWordManagement.Module
{
    /// <summary>
    /// Interaction logic for EditActionword.xaml
    /// </summary>
    public partial class EditActionword : Window
    {
        private Actionword actionword;
        public Actionword CurrentActionword { get { return actionword; } set { actionword = value; FillArgumentCollection(); Details.DataContext = actionword; } }

        ObservableCollection<Argument> _ArgumentCollection =
            new ObservableCollection<Argument>();

        public ObservableCollection<Argument> ArgumentCollection
            { get { return _ArgumentCollection; } }

        private void FillArgumentCollection()
        {
            foreach (var item in actionword.Arguments)
            {
                _ArgumentCollection.Add(item);
            }
        }


        public EditActionword()
        {
            InitializeComponent();
            lstArguments.DataContext = this;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (actionword != null)
            {
                try
                {
                    actionword.Save();
                }
                catch (Exception)
                {
                    Globals.ShowMessage("Cannot save actionword. Please make sure the actionword is unique.");
                    return;
                }
                
            }
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnArgMoveUp_Click(object sender, RoutedEventArgs e)
        {
            // only move if selected and not the first one
            if (lstArguments.SelectedIndex <= 0) return;

            var idx = lstArguments.SelectedIndex;

            _ArgumentCollection.Move(idx, idx - 1);
            _ArgumentCollection[idx-1].Number -= 1;
            _ArgumentCollection[idx].Number += 1;
        }

        private void btnArgMoveDown_Click(object sender, RoutedEventArgs e)
        {
            // only move if selected and not the last one
            if (lstArguments.SelectedIndex == -1) return;
            if (lstArguments.SelectedIndex >= lstArguments.Items.Count-1) return;

            var idx = lstArguments.SelectedIndex;

            _ArgumentCollection.Move(idx, idx + 1);
            _ArgumentCollection[idx].Number -= 1;
            _ArgumentCollection[idx+1].Number += 1;
        }

        private void btnSelectCategory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SelectCategory();
            var elem_id = CurrentActionword.Element.ID;
            dlg.SelectedElement = CurrentActionword.Element;
            Nullable<bool> dialogResult = dlg.ShowDialog();
            if (dialogResult == true)
            {
                if (elem_id != dlg.SelectedElement.ID)
                {
                    CurrentActionword.Element = dlg.SelectedElement;
                }

            }

        }

    }
}
