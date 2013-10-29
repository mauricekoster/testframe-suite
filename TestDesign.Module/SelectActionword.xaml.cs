
namespace TestDesign.Module
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
    using Cluster.Presenter;

    /// <summary>
    /// Interaction logic for SelectActionword.xaml
    /// </summary>
    public partial class SelectActionword : Window
    {
        public SelectActionword()
        {
            InitializeComponent();
            FillList();
        }

        private void FillList()
        {
            var list = Element.GetList();

            foreach (var l in list)
            {
                TreeViewItem newChild = new TreeViewItem();
                newChild.Header = l.Name;
                newChild.Tag = l;
                newChild.IsExpanded = true;
                treeView1.Items.Add(newChild);

                FillList(newChild, l);
                
            }

            // TODO: Actionwords not linked to element (=0)?

        }

        private void FillList(TreeViewItem parent, Element element)
        {
            var list = Element.GetList( element.ID );
            if (list.Count() > 0)
            {
                foreach (var l in list)
                {
                    TreeViewItem newSubChild = new TreeViewItem();
                    newSubChild.Header = l.Name;
                    newSubChild.Tag = l;
                    newSubChild.IsExpanded = true;
                    parent.Items.Add(newSubChild);

                    FillList(newSubChild, l);
                }
            }
           
            foreach (var aw in element.Actionwords)
            {
                TreeViewItem newSubChild = new TreeViewItem();
                newSubChild.Header = aw.Name;
                newSubChild.FontWeight = FontWeights.Bold;
                newSubChild.Tag = aw;
                newSubChild.IsExpanded = true;
                newSubChild.MouseDoubleClick += new MouseButtonEventHandler(newSubChild_MouseDoubleClick);
                parent.Items.Add(newSubChild);

            }
        }

        void newSubChild_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (TreeViewItem)sender;
            var aw = (Actionword)item.Tag;

            TestFrame.InsertActionwordCurrentRow(aw);

            this.Close();   
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            var sel = (TreeViewItem)treeView1.SelectedItem;

            if (sel == null) return; // nothing selected

            var t = sel.Tag;
            
            
            if (t == null) return;

            if ( t.GetType().Equals(typeof(Actionword)))
            {
                var aw = (Actionword)t;

                TestFrame.InsertActionwordCurrentRow(aw);

                this.Close();                
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }
}
