namespace ActionwordManagement.Module
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
    using General.Presenter;

    /// <summary>
    /// Interaction logic for ManageActionwords.xaml
    /// </summary>
    /// 

    public partial class ManageActionwords : Window
    {
        bool elementEdit = false;
        private Actionword current_actionword = new Actionword() { Name = "No name" };
        private Element current_element = new Element() { Name = "No name" };
        private Argument current_argument = new Argument() { Name = "No name" };

        public ManageActionwords()
        {
            InitializeComponent();
            FillElementTree();

            

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private bool IsElementSelected()
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;
            var t = sel.Tag;
            if (t == null) return false;

            return (t.GetType().Equals(typeof(Element)));
        }

        private void btnEditElement_Click(object sender, RoutedEventArgs e)
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;
            if (sel == null) return; // nothing selected           
            var t = sel.Tag;

            ToggleEditMode(sel, t);
        }

        private void ToggleEditMode(TreeViewItem sel, object t)
        {
            if (!elementEdit)
            {
                // enable element edit controls
                tvElements.IsEnabled = false;
                btnAddElement.IsEnabled = false;
                btnAddRootElement.IsEnabled = false;
                btnAddActionword.IsEnabled = false;
                btnRemoveElement.IsEnabled = false;

                if (IsElementSelected())
                    ElementEditor.IsEnabled = true;
                else
                {
                    ActionwordDetails.IsEnabled = true;
                    ActionwordDescription.IsEnabled = true;
                    ActionwordScreens.IsEnabled = true;
                    ArgumentDetails.IsEnabled = true;
                    stackPanelArgumentButtons.IsEnabled = true;
                }

                btnEditElement.Content = "Done";
            }
            else
            {
                // enable element edit controls
                tvElements.IsEnabled = true;
                btnAddElement.IsEnabled = true;
                btnAddRootElement.IsEnabled = true;
                btnAddActionword.IsEnabled = true;
                btnRemoveElement.IsEnabled = true;

                if (IsElementSelected())
                    ElementEditor.IsEnabled = false;
                else
                {
                    ActionwordDetails.IsEnabled = false;
                    ActionwordDescription.IsEnabled = false;
                    ActionwordScreens.IsEnabled = false;
                    ArgumentDetails.IsEnabled = false;
                    stackPanelArgumentButtons.IsEnabled = false;
                }


                btnEditElement.Content = "Edit";

                if (t.GetType().Equals(typeof(Element)))
                {
                    sel.Header = ((Element)t).Name;
                }
                else
                {
                    sel.Header = ((Actionword)t).Name;
                }
                //FillElementTree();
            }
            elementEdit = !elementEdit;
        }

        #region Fill trees and lists

        private TreeViewItem AddElementToTree(TreeViewItem parent, Element element)
        {
            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = element.Name;
            newChild.Tag = element;
            newChild.IsExpanded = true;
            if (parent == null)
                tvElements.Items.Add(newChild);
            else
                parent.Items.Add(newChild);

            return newChild;
        }

        private TreeViewItem AddActionwordToTree(TreeViewItem parent, Actionword aw)
        {
            TreeViewItem newSubChild = new TreeViewItem();
            newSubChild.Header = aw.Name;
            newSubChild.FontWeight = FontWeights.Bold;
            newSubChild.Tag = aw;
            newSubChild.IsExpanded = true;
            parent.Items.Add(newSubChild);
            return newSubChild;
        }

        private ListBoxItem AddArgumentToList(Argument arg)
        {
            var item = new ListBoxItem();
            //item.Name = a.Name;
            item.Content = arg.Name;
            item.Tag = arg;
            lstArguments.Items.Add(item);
            
            return item;
        }

        private void FillElementTree()
        {
            var list = Element.GetList();
            tvElements.Items.Clear();

            foreach (var l in list)
            {
                var newChild = AddElementToTree(null, l);

                FillElementTree(newChild, l);

            }

            // TODO: Actionwords not linked to element (=0)?

        }

        private void FillElementTree(TreeViewItem parent, Element element)
        {
            foreach (var l in element.Elements)
            {
                var newSubChild = AddElementToTree(parent, l);
                FillElementTree(newSubChild, l);
            }

            foreach (var aw in element.Actionwords)
            {
                AddActionwordToTree(parent, aw);
            }
        }

        #endregion

        private void tvElements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;


            if (t == null) return;

            if (t.GetType().Equals(typeof(Element)))
            {
                current_actionword = null;
                current_element = (Element)t;
            }
            else if (t.GetType().Equals(typeof(Actionword)))
            {
                current_actionword = (Actionword)t;
                current_element = current_actionword.Element;
            }

            stackPanelElement.DataContext = current_element;
            stackPanelActionword.DataContext = current_actionword;
            

            FillScreens();
            FillArguments();
        }

        private void FillArguments()
        {
            lstArguments.Items.Clear();
            if (current_actionword == null) return;

            foreach (var a in current_actionword.Arguments)
            {
                var item = new ListBoxItem();
                //item.Name = a.Name;
                item.Content = a.Name;
                item.Tag = a;
                lstArguments.Items.Add(item);
            }
            stackPanelArgument.DataContext = null;
        }

        private void FillScreens()
        {
            cmbActionwordStartScreen.Items.Clear();
            if (current_actionword == null) return;

            var item = new ComboBoxItem();
            item.Content = "<no screen>";
            item.IsSelected = (0 == current_actionword.StartScreen);
            item.Tag = null;
            cmbActionwordStartScreen.Items.Add(item);

            foreach (var s in current_element.Screens)
            {
                item = new ComboBoxItem();
                item.Content = s.Name;
                item.IsSelected = (s.ID == current_actionword.StartScreen);
                item.Tag = s;
                cmbActionwordStartScreen.Items.Add( item );
            }
        }

       

        private void cmbActionwordStartScreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = (ComboBoxItem)cmbActionwordStartScreen.SelectedItem;
            if (sel == null) return;

            var tag = (Screen)sel.Tag;
            if (tag==null)
                current_actionword.StartScreen = 0;
            else
                current_actionword.StartScreen = tag.ID;

        }

        private void lstArguments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = (ListBoxItem)lstArguments.SelectedItem;
            if (sel == null) return;

            
            var tag = (Argument)sel.Tag;
            current_argument = tag;

            stackPanelArgument.DataContext = current_argument;
        }

        

        private void btnAddRootElement_Click(object sender, RoutedEventArgs e)
        {
            string name = Globals.InputBox("Enter name for new element:",  "<new element>");
            if (name == "") return;

            var element = Element.New(null, name);

            var tvi = AddElementToTree(null, element);
            tvi.IsSelected = true;

            ToggleEditMode(tvi, element);

            txtElementFullname.Focus();
        }

        private void btnAddElement_Click(object sender, RoutedEventArgs e)
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;

            if (t == null) return;

            if (t.GetType().Equals(typeof(Element)))
            {
                string name = Globals.InputBox("Enter name for new element:", "<new element>");
                if (name == "") return;
                var elem = (Element)t;
                var element = Element.New(elem, name);
                var tvi = AddElementToTree(sel, element);
                tvi.IsSelected = true;

                ToggleEditMode(tvi, element);

                txtElementFullname.Focus();
            }
            
        }

        private void btnAddActionword_Click(object sender, RoutedEventArgs e)
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;
            TreeViewItem parent = null;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;

            if (t == null) return;

            if (t.GetType().Equals(typeof(Element)))
            {
                parent = sel;
            }
            else
            {
                // if actionword, then add new actionword in same element as selected actionword
                parent = (TreeViewItem)sel.Parent;         
            }

            string name = Globals.InputBox("Enter name for new actionword:", "<new actionword>" );
            if (name == "") return;
            var actionword = Actionword.New(current_element, name);
            var tvi = AddActionwordToTree(parent, actionword);
            tvi.IsSelected = true;

            ToggleEditMode(tvi, actionword);

            txtActionwordDescription.Focus();
        }

        private void btnNewArgument_Click(object sender, RoutedEventArgs e)
        {
            string name = Globals.InputBox("Enter name for new argument:",  "<new argument>");
            if (name == "") return;
            var argument = Argument.New(current_actionword, name);
            
            var item = AddArgumentToList(argument);
            item.IsSelected = true;
        }

        private void btnRemoveElement_Click(object sender, RoutedEventArgs e)
        {
            var sel = (TreeViewItem)tvElements.SelectedItem;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;


            if (t == null) return;

            if (t.GetType().Equals(typeof(Element)))
            {
                var element = (Element)t;

                if (element.Elements.Count() > 0 || element.Actionwords.Count() > 0 || element.Screens.Count() > 0)
                {
                    if ( !Globals.YN("Element still has subelements, actionwords or screens. Are you sure to remove them all?" ) )
                        return;
                }
                
                element.Remove();
            }
            else
            {
                var actionword = (Actionword)t;

                if (MessageBox.Show(String.Format("Are you sure to remove the '{0}' action?", actionword.Name),
                        Globals.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                
                actionword.Remove();
            }

            var parent = sel.Parent;
            if (parent.GetType().Equals(typeof(TreeView)))
            {
                var tv = (TreeView)parent;
                tv.Items.Remove(sel);
            }
            else
            {
                var tv = (TreeViewItem)parent;
                tv.Items.Remove(sel);
            }
            
        }

       

       

    }
}
