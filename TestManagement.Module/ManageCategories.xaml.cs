namespace TestManagement.Module
{

    using System.Windows;
    using System.Windows.Controls;
    using ActionWord.Presenter;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for ManageCategories.xaml
    /// </summary>
    public partial class ManageCategories : Window
    {
        private Element current_element = null;
        private List<Element> _list;

        public ManageCategories()
        {
            InitializeComponent();
            FillElementTree();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }



        private void FillElementTree()
        {
            _list = Element.GetList();
            tvCategories.Items.Clear();

            foreach (var l in _list)
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            foreach (var l in _list)
            {
                l.Save();

            }

            this.DialogResult = true;
            this.Close();
        }

    }
}
