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

namespace ActionWordManagement.Module
{
    /// <summary>
    /// Interaction logic for SelectCategory.xaml
    /// </summary>
    public partial class SelectCategory : Window
    {
        private Element current_element = null;
        public Element SelectedElement { get { return current_element; }
            set
            {
                var e = (TreeViewItem)tvCategories.FindName(value.Name.Replace(" ", "_"));
                e.IsSelected = true;
                current_element = (Element)e.Tag;
            }
        }

        public SelectCategory()
        {
            InitializeComponent();
            FillElementTree();
        }

        private void FillElementTree()
        {
            var list = Element.GetList();
            tvCategories.Items.Clear();

            foreach (var l in list)
            {
                var newChild = AddElementToTree(null, l);

                FillElementTree(newChild, l);

            }
        }

        private void FillElementTree(TreeViewItem parent, Element element)
        {
            foreach (var l in element.Elements)
            {
                var newSubChild = AddElementToTree(parent, l);
                FillElementTree(newSubChild, l);
            }
            
        }

        private TreeViewItem AddElementToTree(TreeViewItem parent, Element element)
        {
            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = element.Name;
            newChild.Tag = element;
            newChild.IsExpanded = true;
            if (parent == null)
                tvCategories.Items.Add(newChild);
            else
                parent.Items.Add(newChild);

            tvCategories.RegisterName(element.Name.Replace(" ", "_"), newChild);

            return newChild;
        }

        private void tvCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sel = (TreeViewItem)tvCategories.SelectedItem;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;

            if (t == null) return;

            if (t.GetType().Equals(typeof(Element)))
            {
                current_element = (Element)t;
            }
        }

        private void tvCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

    }
}
