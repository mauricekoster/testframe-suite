

namespace ActionWordManagement.Module
{
    using System.Windows;
    using ActionWord.Presenter;
    using System.Windows.Controls;
    using ActionwordManagement.Module;

    /// <summary>
    /// Interaction logic for ManageActionwords_TopLevel.xaml
    /// </summary>
    public partial class ManageActionwords_TopLevel : Window
    {
        private Actionword current_actionword = new Actionword() { Name = "No name" };
        private Element current_element = new Element() { Name = "No name" };

        public ManageActionwords_TopLevel()
        {
            InitializeComponent();
            FillElementTree();
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
            current_element = list[0];

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

            tvElements.RegisterName(element.Name.Replace(" ", "_"), newChild);

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

            tvElements.RegisterName(aw.Name.Replace(" ","_"), newSubChild);

            return newSubChild;
        }

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

            txtDescription.DataContext = current_actionword;

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var aw = Actionword.New(current_element, "new actionword");
            ActionwordManagementActions.ShowEditActionword(aw);
            MoveActionword(aw, null);
        }

        private void MoveActionword(Actionword aw, string old_element)
        {
            TreeViewItem e_from = null;
            if (old_element!=null) 
                e_from = (TreeViewItem)tvElements.FindName(old_element.Replace(" ", "_"));

            TreeViewItem e_to = (TreeViewItem)tvElements.FindName(aw.Element.Name.Replace(" ", "_"));
            TreeViewItem e_aw = (TreeViewItem)tvElements.FindName(aw.Name.Replace(" ", "_"));

            if (old_element == null)
            {
                // new action, just add to element
                var dummy = AddActionwordToTree(e_to, aw);
            }
            else
            {
                if(e_from != null) e_from.Items.Remove(e_aw);
                if(e_to!=null) e_to.Items.Add(e_aw);
            }

        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            var elem_name = current_actionword.Element.Name;
            var dlg_result = ActionwordManagementActions.ShowEditActionword(current_actionword);
            if (dlg_result == false) return;

            if (elem_name != current_actionword.Element.Name)
            {
                MoveActionword(current_actionword, elem_name);
            }
        }
    }
}
